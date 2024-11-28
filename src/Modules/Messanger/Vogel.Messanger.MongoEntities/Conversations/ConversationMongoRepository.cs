using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Messanger.MongoEntities.Users;
using MongoDB.Driver.Linq;
namespace Vogel.Messanger.MongoEntities.Conversations
{
    public class ConversationMongoRepository : MongoRepository<ConversationMongoEntity>
    {
        public ConversationMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }

        public async Task<Paging<ConversationMongoView>> QueryViewAsync(string userId, string? cursor, int limit = 10, bool ascending = false)
        {
            var query = await BuildViewQuery(userId);

            return await query.ToPaged(cursor, limit, ascending);
        }

        public async Task<ConversationMongoView?> FindViewAsync(string userId, string conversationId)
        {
            var query = await BuildViewQuery(userId);

            return await query.Match(x => x.Id == conversationId).SingleOrDefaultAsync();
        }

        public async Task<UpdateResult> InsertParticipantAsync(string conversationId, ParticipantMongoEntity entity)
        {
            var filter = Filter.Eq(x => x.Id, conversationId);
            var update = Update.Push(x => x.Participants, entity);
            return await UpdateAsync(filter, update);
        }

        public async Task<UpdateResult> UpdateParticipantAsync(string conversationId, ParticipantMongoEntity participant)
        {
            var filter = Filter.And(
                    Filter.Eq(x => x.Id, conversationId),
                    Filter.Eq(x => x.Participants.First().Id, participant.Id)
                );

            var update = Update.Set(x => x.Participants.First().UserId, participant.UserId)
                .Set(x => x.Participants.First().CreatorId, participant.CreatorId)
                .Set(x => x.Participants.First().CreationTime, participant.CreationTime)
                .Set(x => x.Participants.First().ModifierId, participant.ModifierId)
                .Set(x => x.Participants.First().ModificationTime, participant.ModificationTime)
                .Set(x => x.Participants.First().DeletorId, participant.DeletorId)
                .Set(x => x.Participants.First().DeletionTime, participant.DeletionTime);

            return await UpdateAsync(filter, update);
        }

        public async Task<UpdateResult> RemoveParticipantAsync(string conversationId, string participantId)
        {
            var filter = Filter.Eq(x => x.Id, conversationId);

            var update = Update.PullFilter(
                    x => x.Participants,
                    Builders<ParticipantMongoEntity>.Filter.Eq(x => x.Id, participantId)
                );

            return await UpdateAsync(filter, update);
        }

        public async Task<Paging<ParticipantMongoEntity>> QueryParticipantAsync(string conversationId , string? cursor, int limit = 10, bool ascending = false)
        {
            var query = await BuildParticipantQuery(conversationId);

            return await query.ToPaged(cursor, limit, ascending);
        }

        public async Task<ParticipantMongoEntity?> FindParticipantAsync(string conversationId , string participantId)
        {
            var query = await BuildParticipantQuery(conversationId);

            return await query.Match(x => x.Id == participantId).SingleOrDefaultAsync();
        }

        private async Task<IAggregateFluent<ConversationMongoView>> BuildViewQuery(string userId)
        {
            var query = MongoDbCollection.Aggregate()
                 .Match(x => x.Participants.Any(x => x.UserId == userId))
                 .Unwind(x=> x.Participants , new AggregateUnwindOptions<ConversationJoinedView> { PreserveNullAndEmptyArrays = true})
                 .Lookup<ConversationJoinedView, UserMongoEntity , ConversationJoinedView>(
                    GetCollection<UserMongoEntity>(UserMongoConsts.CollectionName),
                    x=> x.Participants.UserId,
                    c=> c.Id,
                    x=> x.Participants.User
                 )
                 .Unwind(x=> x.Participants.User, new AggregateUnwindOptions<ConversationJoinedView> { PreserveNullAndEmptyArrays = true})
                 .Group(c => c.Id, x => new ConversationMongoView
                 {
                     Id = x.Key,
                     Name = x.First().Name ??  
                     x.Select(x=> x.Participants).Where(x=> x.UserId != userId).First().User.FirstName
                     + " "
                     + x.Select(x => x.Participants).Where(x => x.UserId != userId).First().User.LastName,
                     TotalParticpants = x.Select(x=> x.Participants).Count(),
                     Avatar = x.Select(x => x.Participants).Where(x => x.UserId != userId).First().User.Avatar.File,
                     Participants = x.Select(x=> x.Participants).Take(10).ToList(),
                     CreatorId = x.First().CreatorId,
                     CreationTime = x.First().CreationTime,
                     ModifierId = x.First().ModifierId,
                     ModificationTime = x.First().ModificationTime,
                     DeletionTime = x.First().DeletionTime,
                     DeletorId = x.First().DeletorId,
                     
                 });

            return query;
        }

        private async Task<IAggregateFluent<ParticipantMongoEntity>> BuildParticipantQuery(string conversationId)
        {
            var query = MongoDbCollection.Aggregate()
                .Match(x => x.Id == conversationId)
                .Unwind<ConversationMongoEntity, ConversationJoinedView>(x => x.Participants)
                .Lookup<ConversationJoinedView, UserMongoEntity, ConversationJoinedView>(
                    GetCollection<UserMongoEntity>(UserMongoConsts.CollectionName),
                    x => x.Participants.UserId,
                    f => f.Id,
                    x => x.Participants.User
                )
                .Unwind(x => x.Participants.User, new AggregateUnwindOptions<ConversationJoinedView> { PreserveNullAndEmptyArrays = true })
                .ReplaceRoot(x => x.Participants);

            return query;
        }
    }
}

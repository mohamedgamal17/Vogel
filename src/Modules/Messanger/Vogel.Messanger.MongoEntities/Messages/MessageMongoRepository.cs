using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Messanger.MongoEntities.Conversations;
using Vogel.Messanger.MongoEntities.Users;
namespace Vogel.Messanger.MongoEntities.Messages
{
    public class MessageMongoRepository : MongoRepository<MessageMongoEntity>
    {
        private readonly IMongoRepository<UserMongoEntity> _userMongoRepository;
        public MessageMongoRepository(IMongoDatabase mongoDatabase, IMongoRepository<UserMongoEntity> userMongoRepository) : base(mongoDatabase)
        {
            _userMongoRepository = userMongoRepository;
        }

        public async Task<Paging<MessageMongoView>> QueryViewAsync(string conversationId , string? cursor = null, bool ascending = false, int limit = 10)
        {
            var query = await BuildViewQuery(conversationId);

            return await query.ToPaged(cursor, limit, ascending);
        }

        public async Task<MessageMongoView?> FindViewAsync(string conversationId,  string messageId)
        {
            var query = await BuildViewQuery(conversationId);

            return await query.Match(x => x.Id == messageId).SingleOrDefaultAsync();
        }

        public async Task<UpdateResult> LogConversationMessages(string conversationId ,string userId , DateTime seenAt )
        {
            var filter = Filter.Where(x => x.ConversationId == conversationId
                && x.SenderId != userId
                && !x.Logs.Any(x => x.SeenById == userId));

            var log = new MessageLogMongoEntity
            {
                Id = Guid.NewGuid().ToString(),
                SeenById = userId,
                SeenAt = seenAt,
                CreationTime = DateTime.UtcNow,
                ModificationTime = DateTime.UtcNow
            };

            var update = Update.Push(x => x.Logs, log);

           return await MongoDbCollection.UpdateManyAsync(filter, update);
        }

        public IAggregateFluent<MessageMongoView> GetMessageViewAsAggregate()
        {
            var query = AsMongoCollection()
                .Aggregate()
                .Lookup<MessageMongoEntity, UserMongoEntity, MessageMongoView>(
                    _userMongoRepository.AsMongoCollection(),
                    l => l.SenderId,
                    f => f.Id,
                    @as => @as.Sender
                )
                .Unwind(x=> x.Sender , new AggregateUnwindOptions<MessageMongoView>() { PreserveNullAndEmptyArrays = true });

            return query;
        }


        private async Task<IAggregateFluent<MessageMongoView>> BuildViewQuery(string conversationId)
        {
            var query = MongoDbCollection.Aggregate()
                .Match(x=> x.ConversationId == conversationId)
                .Lookup<MessageMongoEntity, ConversationMongoEntity, MessageJoinedView>(
                    GetCollection<ConversationMongoEntity>(ConversationConsts.ConversationCollection),
                    x => x.ConversationId,
                    f => f.Id,
                    x => x.Conversation
                )
                .Unwind<MessageJoinedView, MessageJoinedView>(x => x.Conversation)
                .Lookup<MessageJoinedView, UserMongoEntity, MessageJoinedView>(
                    GetCollection<UserMongoEntity>(UserMongoConsts.CollectionName),
                    l => l.SenderId,
                    f => f.Id,
                    x => x.Sender
                )
                .Unwind<MessageJoinedView, MessageJoinedView>(x => x.Sender)
                .Project(x => new MessageMongoView
                {
                    Id = x.Id,
                    SenderId = x.SenderId,
                    Sender = x.Sender,
                    Content = x.Content,
                    ConversationId = x.ConversationId,
                    IsSeen = x.Logs.Count == x.Conversation.Participants.Count - 1,
                    CreatorId = x.CreatorId,
                    CreationTime = x.CreationTime,
                    ModifierId = x.ModifierId,
                    ModificationTime = x.ModificationTime,
                    DeletorId = x.DeletorId,
                    DeletionTime = x.DeletionTime
                });


            return query;
        } 
    }
}

using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Messanger.MongoEntities.Users;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Core.Operations;
namespace Vogel.Messanger.MongoEntities.Conversations
{
    public class ConversationMongoRepository : MongoRepository<ConversationMongoEntity>
    {
        private readonly IMongoRepository<UserMongoEntity> _userMongoRepository;
        public ConversationMongoRepository(IMongoDatabase mongoDatabase , IMongoRepository<UserMongoEntity> userMongoRepository) : base(mongoDatabase)
        {
            _userMongoRepository = userMongoRepository;
        }

        public async Task<Paging<ConversationMongoView>> GetUserPagedConversationView(string userId , string? cursor = null , bool ascending = false, int limit = 10)
        {
            var query = await GetConversationViewAsAggregate()
                .Match(Builders<ConversationMongoView>.Filter.Eq(x => x.Participants.First().UserId, userId))
                .ToPaged(cursor, limit, ascending);

            return query;
        }

        public async Task<ConversationMongoView> GetConversationViewById(string id)
        {
            return await GetConversationViewAsAggregate()
                .Match(x => x.Id == id)
                .SingleOrDefaultAsync();
        }

        public IAggregateFluent<ConversationMongoView> GetConversationViewAsAggregate()
        {
            var aggregate = AsMongoCollection()
                .Aggregate()
                .Lookup<ConversationMongoEntity, ParticipantMongoView, ConversationUngroupedMongoView>(
                    GetCollection<ParticipantMongoView>(ConversationConsts.ParticipantCollection),
                    l => l.Id,
                    f => f.ConversationId,
                    @as => @as.Participant
                )
                .Unwind(x => x.Participant, new AggregateUnwindOptions<ConversationUngroupedMongoView>() { PreserveNullAndEmptyArrays = true })
                .Lookup<ConversationUngroupedMongoView, UserMongoEntity, ConversationUngroupedMongoView>(
                    _userMongoRepository.AsMongoCollection(),
                    l => l.Participant.UserId,
                   f => f.Id,
                   @as => @as.Participant.User
                )
                .Unwind(x=> x.Participant.User, new AggregateUnwindOptions<ConversationUngroupedMongoView>() {  PreserveNullAndEmptyArrays = true})
                .Group(x => x.Id, grouped => new ConversationMongoView
                {
                    Id = grouped.Key,
                    Name = grouped.First().Name,
                    Participants = grouped.Select(x => x.Participant).ToList(),
                    CreatorId = grouped.First().CreatorId,
                    CreationTime = grouped.First().CreationTime,
                    ModifierId = grouped.First().ModifierId,
                    ModificationTime = grouped.First().ModificationTime,
                    DeletionTime = grouped.First().DeletionTime,
                    DeletorId = grouped.First().DeletorId
                });
            return aggregate;
        }
    }
}

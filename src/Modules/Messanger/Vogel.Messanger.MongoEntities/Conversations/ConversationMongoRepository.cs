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
                .Lookup<ConversationMongoEntity, ParticipantMongoView, ConversationMongoView>(
                    GetCollection<ParticipantMongoView>(ConversationConsts.ParticipantCollection),
                    l => l.Id,
                    f => f.ConversationId,
                    @as => @as.Participants
                )
                .Lookup<ConversationMongoView, UserMongoEntity, ConversationMongoView>(
                   _userMongoRepository.AsMongoCollection(),
                   l => l.Participants.First().UserId,
                   f => f.Id,
                   @as => @as.Participants.First().User
                );

            return aggregate;
        }
    }
}

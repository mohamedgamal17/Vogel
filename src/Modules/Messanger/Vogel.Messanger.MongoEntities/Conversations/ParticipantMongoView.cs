using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Messanger.MongoEntities.Users;
namespace Vogel.Messanger.MongoEntities.Conversations
{
    public class ParticipantMongoView : FullAuditedMongoEntity<string>
    {
        public string UserId { get; set; }
        public string ConversationId { get; set; }
        public UserMongoEntity User { get; set; }
    }

    public class ParticipantMongoRepository : MongoRepository<ParticipantMongoEntity>
    {
        private readonly IMongoRepository<UserMongoEntity> _userMongoRepository;
        public ParticipantMongoRepository(IMongoDatabase mongoDatabase, IMongoRepository<UserMongoEntity> userMongoRepository) : base(mongoDatabase)
        {
            _userMongoRepository = userMongoRepository;
        }

        public async Task<Paging<ParticipantMongoView>> GetConversationParticipantsPaged(string conversationId , string? cursor = null , int limit = 10)
        {
            var query = PrepareParticipantMongoViewAggregate(conversationId);

            return await query.ToPaged(cursor, limit);
        }

        public async Task<ParticipantMongoView> GetConversationParticipant(string conversationId ,string participantId)
        {
            var result = await PrepareParticipantMongoViewAggregate(conversationId)
                .Match(Builders<ParticipantMongoView>.Filter.Eq(x => x.Id, participantId))
                .SingleOrDefaultAsync();

            return  result;
        }

        public async Task<bool> IsUserParticipantInConversation(string conversationId,string userId )
        {
            return await AsQuerable().AnyAsync(x => x.ConversationId == conversationId && x.UserId == userId);
        }

        private  IAggregateFluent<ParticipantMongoView> PrepareParticipantMongoViewAggregate(string? conversationId = null)
        {
            var aggregate = AsMongoCollection()
                .Aggregate()
                .Lookup<ParticipantMongoEntity, UserMongoEntity, ParticipantMongoView>(_userMongoRepository.AsMongoCollection(),
                    l => l.UserId,
                    f => f.Id,
                    @as => @as.User
                )
                .Unwind(x => x.User, new AggregateUnwindOptions<ParticipantMongoView> { PreserveNullAndEmptyArrays = true });

            if(conversationId != null)
            {
                aggregate = aggregate.Match(Builders<ParticipantMongoView>.Filter.Eq(x => x.ConversationId, conversationId));
            }

            return aggregate;
        }
    }
}

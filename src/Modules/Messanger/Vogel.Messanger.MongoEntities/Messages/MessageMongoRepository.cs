using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
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

        public async Task<Paging<MessageMongoView>> GetPagedMessagesView(string conversationId , string? cursor = null, bool ascending = false, int limit = 10)
        {
            var query = GetMessageViewAsAggregate()
                .Match(
                    Builders<MessageMongoView>.Filter.Eq(x => x.ConversationId, conversationId)
                );

            return await query.ToPaged(cursor, limit, ascending);
        }

        public async Task<MessageMongoView> GetMessageViewbyId(string conversationId,  string messageId)
        {
            return await GetMessageViewAsAggregate()
                .Match(
                    Builders<MessageMongoView>.Filter.And(
                            Builders<MessageMongoView>.Filter.Eq(x => x.ConversationId, conversationId),
                            Builders<MessageMongoView>.Filter.Eq(x => x.Id, messageId)
                        )
                ).SingleOrDefaultAsync();
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
    }
}

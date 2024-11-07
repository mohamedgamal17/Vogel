using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Messanger.MongoEntities.Users;
using MongoDB.Driver.Linq;
using MongoDB.Bson;
using Vogel.Messanger.MongoEntities.Messages;
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
            var query =  GetConversationViewAsAggregate()
                .Match(Builders<ConversationQueryMongoView>.Filter.Eq(x => x.Participants.First().UserId, userId))
                .SubPaged<ConversationQueryMongoView, ConversationMongoView>(x => x.Participants)
                .SubPaged<ConversationMongoView, ConversationMongoView>(x => x.Messages);

            return await query.ToPaged(cursor, limit, ascending);
        }

        public async Task<ConversationMongoView> GetConversationViewById(string id)
        {
            return await GetConversationViewAsAggregate()
                .Match(x => x.Id == id)
                .SubPaged<ConversationQueryMongoView, ConversationMongoView>(x => x.Participants)
                .SubPaged<ConversationMongoView, ConversationMongoView>(x => x.Messages)
                .SingleOrDefaultAsync();
        }

        public IAggregateFluent<ConversationQueryMongoView> GetConversationViewAsAggregate()
        {
            var aggregate = AsMongoCollection()
                .Aggregate()
                .AppendStage<ConversationQueryMongoView>(new BsonDocument("$lookup", new BsonDocument()
                {
                    {"from", ConversationConsts.ParticipantCollection },
                    {"localField", "_id" },
                    {"foreignField" , "conversationId" },
                    {"pipeline", new BsonArray()
                        {
                          {
                            new BsonDocument("$lookup" , new BsonDocument()
                            {
                                {"from" , UserMongoConsts.CollectionName },
                                {"localField", "userId" },
                                {"foreignField", "_id" },
                                {"as" , "user" }
                            })

                          },
                          {
                            new BsonDocument("$unwind", new BsonDocument()
                            {
                                {"path" , "$user" },
                                {"preserveNullAndEmptyArrays" , true }
                            })
                          }

                        }
                    },
                    {"as", "_participants" }

                }))
                .AppendStage<ConversationQueryMongoView>(new BsonDocument("$lookup", new BsonDocument()
                {
                    { "from", MessageMongoConsts.CollectionName },
                    {"localField", "_id" },
                    {"foreignField" , "conversationId" },
                    {"pipeline", new BsonArray()
                        {
                          {
                            new BsonDocument("$lookup" , new BsonDocument()
                            {
                                {"from" , UserMongoConsts.CollectionName },
                                {"localField", "userId" },
                                {"foreignField", "_id" },
                                {"as" , "user" }
                            })

                          },
                          {
                            new BsonDocument("$unwind", new BsonDocument()
                            {
                                {"path" , "$user" },
                                {"preserveNullAndEmptyArrays" , true }
                            })
                          }

                        }
                    },
                    {"as", "_messages" }

                }));



            return aggregate;
        }
    }
}

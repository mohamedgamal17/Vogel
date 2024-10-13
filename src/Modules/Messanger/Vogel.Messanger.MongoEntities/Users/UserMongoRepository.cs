using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.Messanger.MongoEntities.Users
{
    public class UserMongoRepository : MongoRepository<UserMongoEntity>
    {
        public UserMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {

        }
    }
}

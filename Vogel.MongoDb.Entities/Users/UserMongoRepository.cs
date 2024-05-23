using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
namespace Vogel.MongoDb.Entities.Users
{
    public class UserMongoRepository : MongoRepository<UserMongoEntity, string>
    {
        protected override string CollectionName => "users";
        public UserMongoRepository(IMongoDatabase mongoDatabase)
            : base(mongoDatabase)
        {

        }
     
    }
}

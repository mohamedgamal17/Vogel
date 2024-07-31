using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Medias;
namespace Vogel.MongoDb.Entities.Users
{
    public class UserMongoRepository : MongoRepository<UserMongoEntity, string>
    {
        public UserMongoRepository(IMongoDatabase mongoDatabase)
            : base(mongoDatabase)
        {
        }

    }
}

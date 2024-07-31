using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.Friendship
{
    public class FriendRequestMongoRepository : MongoRepository<FriendRequestMongoEntity, string>
    {
        public FriendRequestMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }
    }
}

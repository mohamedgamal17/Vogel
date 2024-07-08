using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.Friendship
{
    public class FriendMongoRepository : MongoRepository<FriendMongoEntity, string>
    {
        protected override string CollectionName => "friends";
        public FriendMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }
    }
}

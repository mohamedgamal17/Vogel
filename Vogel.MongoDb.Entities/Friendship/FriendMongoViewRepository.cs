using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.Friendship
{
    public class FriendMongoViewRepository : MongoRepository<FriendMongoView, string>
    {
        protected override string CollectionName => "friends_view";
        public FriendMongoViewRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }
    }
}

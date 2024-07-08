using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.Friendship
{
    public class FriendRequestMongoViewRepository : MongoRepository<FriendRequestMongoView, string>
    {
        protected override string CollectionName => "friend_requests_view";
        public FriendRequestMongoViewRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }
    }
}

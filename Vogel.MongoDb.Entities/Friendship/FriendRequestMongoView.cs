using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.MongoDb.Entities.Friendship
{
    [MongoCollection("friend_requests_view")]
    public class FriendRequestMongoView : FullAuditedMongoEntity<string>
    {
        public string SenderId { get; set; }
        public string ReciverId { get; set; }
        public PublicUserMongoView Sender { get; set; }
        public PublicUserMongoView Reciver { get; set; }
        public FriendRequestState State { get; set; }
    }
}

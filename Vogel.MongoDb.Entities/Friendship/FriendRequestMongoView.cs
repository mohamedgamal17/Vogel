using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.MongoDb.Entities.Friendship
{
    public class FriendRequestMongoView : FullAuditedMongoEntity<string>
    {
        public string SenderId { get; set; }
        public string ReciverId { get; set; }
        public UserMongoView Sender { get; set; }
        public UserMongoView Reciver { get; set; }
        public FriendRequestState State { get; set; }
    }
}

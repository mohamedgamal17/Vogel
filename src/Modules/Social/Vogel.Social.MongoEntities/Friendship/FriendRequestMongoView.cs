using Vogel.BuildingBlocks.MongoDb;
using Vogel.Social.MongoEntities.Users;
using Vogel.Social.Shared.Common;
namespace Vogel.Social.MongoEntities.Friendship
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

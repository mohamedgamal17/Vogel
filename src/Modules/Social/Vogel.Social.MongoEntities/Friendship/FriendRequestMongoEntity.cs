using Vogel.BuildingBlocks.MongoDb;
using Vogel.Social.Shared.Common;
namespace Vogel.Social.MongoEntities.Friendship
{
    [MongoCollection(FriendshipMongoConsts.FriendRequestCollection)]
    public class FriendRequestMongoEntity : FullAuditedMongoEntity<string>
    {
        public string SenderId { get; set; }
        public string ReciverId { get; set; }
        public FriendRequestState State { get; set; }
    }
}

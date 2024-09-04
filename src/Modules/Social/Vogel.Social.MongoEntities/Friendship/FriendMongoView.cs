using Vogel.BuildingBlocks.MongoDb;
using Vogel.Social.MongoEntities.Users;

namespace Vogel.Social.MongoEntities.Friendship
{
    public class FriendMongoView : FullAuditedMongoEntity<string>
    {
        public string SourceId { get; set; }
        public string TargetId { get; set; }
        public UserMongoView Source { get; set; }
        public UserMongoView Target { get; set; }
    }
}

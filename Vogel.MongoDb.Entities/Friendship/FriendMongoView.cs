using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.MongoDb.Entities.Friendship
{
    public class FriendMongoView : FullAuditedMongoEntity<string>
    {
        public string SourceId { get; set; }
        public string TargetId { get; set; }
        public PublicUserMongoView Source { get; set; }
        public PublicUserMongoView Target { get; set; }
    }
}

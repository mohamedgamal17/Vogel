using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.MongoDb.Entities.Friendship
{
    public class FriendMongoEntity : FullAuditedMongoEntity<string>
    {
        public string SourceId { get; set; }
        public string TargetId { get; set; }

    }
}

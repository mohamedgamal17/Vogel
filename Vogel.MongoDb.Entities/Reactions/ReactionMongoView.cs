using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.MongoDb.Entities.Reactions
{
    public class ReactionMongoView : FullAuditedMongoEntity<string>
    {
        public string UserId { get; set; }
        public string PostId { get; set; }
        public ReactionType Type { get; set; }
        public PublicUserMongoView User { get; set; }
    }
}

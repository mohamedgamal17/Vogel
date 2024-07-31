using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.MongoDb.Entities.PostReactions
{
    [MongoCollection("post_reactions_view")]
    public class PostReactionMongoView : FullAuditedMongoEntity<string>
    {
        public string UserId { get; set; }
        public string PostId { get; set; }
        public ReactionType Type { get; set; }
        public PublicUserMongoView User { get; set; }
    }
}

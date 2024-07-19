using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.PostReactions;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.MongoDb.Entities.CommentReactions
{
    public class CommentReactionMongoView : FullAuditedMongoEntity<string>
    {
        public string UserId { get; set; }
        public string CommentId { get; set; }
        public ReactionType Type { get; set; }
        public PublicUserMongoView User { get; set; }
    }
}

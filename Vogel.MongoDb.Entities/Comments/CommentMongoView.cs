using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.CommentReactions;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.MongoDb.Entities.Comments
{
    [MongoCollection("comments_view")]
    public class CommentMongoView : FullAuditedMongoEntity<string>
    {
        public string Content { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
        public string? CommentId { get; set; }
        public PublicUserMongoView User { get; set; }
        public CommentReactionSummaryMongoView ReactionSummary { get; set; }
    }
}

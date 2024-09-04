using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.MongoEntities.CommentReactions;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.Content.MongoEntities.Comments
{
    public class CommentMongoView : FullAuditedMongoEntity<string>
    {
        public string Content { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
        public string? CommentId { get; set; }
        public UserMongoView User { get; set; }
        public CommentReactionSummaryMongoView ReactionSummary { get; set; }
    }
}

using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.MongoEntities.PostReactions;
namespace Vogel.Content.MongoEntities.CommentReactions
{
    public class CommentReactionMongoView : FullAuditedMongoEntity<string>
    {
        public string UserId { get; set; }
        public string CommentId { get; set; }
        public ReactionType Type { get; set; }
    }
}

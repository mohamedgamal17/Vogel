using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.MongoEntities.Medias;
using Vogel.Content.MongoEntities.PostReactions;
namespace Vogel.Content.MongoEntities.Posts
{
    public class PostMongoView : FullAuditedMongoEntity<string>
    {
        public string Caption { get; set; }
        public string? MediaId { get; set; }
        public string UserId { get; set; }
        public MediaMongoEntity? Media { get; set; }
        public PostReactionSummaryMongoView? ReactionSummary { get; set; }
    }
}

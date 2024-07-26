using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Medias;
using Vogel.MongoDb.Entities.PostReactions;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.MongoDb.Entities.Posts
{
    public class PostMongoView : FullAuditedMongoEntity<string>
    {
        public string Caption { get; set; }
        public string? MediaId { get; set; }
        public string UserId { get; set; }
        public PublicUserMongoView User { get; set; }
        public MediaMongoEntity? Media { get; set; }

        public PostReactionSummaryMonogView ReactionSummary { get; set; }
    }
}

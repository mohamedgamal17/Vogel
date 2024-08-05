using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.MongoDb.Entities.PostReactions
{
    [MongoCollection(PostReactionMongoConsts.ReactionSummaryView)]
    public class PostReactionMongoView : FullAuditedMongoEntity<string>
    {
        public string UserId { get; set; }
        public string PostId { get; set; }
        public ReactionType Type { get; set; }
        public UserMongoView User { get; set; }
    }
}

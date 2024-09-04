using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.Content.MongoEntities.CommentReactions
{
    public class CommentReactionSummaryMongoView : MongoEntity
    {
        public long TotalLike { get; set; }
        public long TotalLove { get; set; }
        public long TotalLaugh { get; set; }
        public long TotalSad { get; set; }
        public long TotalAngry { get; set; }
    }
}

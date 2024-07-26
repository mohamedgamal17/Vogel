
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.CommentReactions
{
    public class CommentReactionSummaryMongoView : MongoEntity<string>
    {
        public long TotalLike { get; set; }
        public long TotalLove { get; set; }
        public long TotalLaugh { get; set; }
        public long TotalSad { get; set; }
        public long TotalAngry { get; set; }
    }
}

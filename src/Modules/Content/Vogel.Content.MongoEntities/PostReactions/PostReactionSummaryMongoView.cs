using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.Content.MongoEntities.PostReactions
{
    public class PostReactionSummaryMongoView : MongoEntity
    {
        public long TotalLike { get; set; }
        public long TotalLove { get; set; }
        public long TotalLaugh { get; set; }
        public long TotalSad { get; set; }
        public long TotalAngry { get; set; }

    }
}

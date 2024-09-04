using Vogel.BuildingBlocks.Shared.Dtos;

namespace Vogel.Content.Application.PostReactions.Dtos
{
    public class PostReactionSummaryDto : EntityDto<string>
    {
        public long TotalLike { get; set; }
        public long TotalLove { get; set; }
        public long TotalLaugh { get; set; }
        public long TotalSad { get; set; }
        public long TotalAngry { get; set; }
    }
}

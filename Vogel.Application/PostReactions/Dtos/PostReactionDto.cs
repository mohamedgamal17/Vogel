using Vogel.BuildingBlocks.Application.Dtos;
using Vogel.Domain.Posts;

namespace Vogel.Application.PostReactions.Dtos
{
    public class PostReactionDto : EntityDto<string>
    {
        public string UserId { get; set; }
        public string PostId { get; set; }
        public ReactionType Type { get; set; }
    }
}

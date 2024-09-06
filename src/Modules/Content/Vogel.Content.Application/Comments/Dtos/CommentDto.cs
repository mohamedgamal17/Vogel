using Vogel.BuildingBlocks.Shared.Dtos;
using Vogel.Content.Application.CommentReactions.Dtos;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Content.Application.Comments.Dtos
{
    public class CommentDto : EntityDto<string>
    {
        public string Content { get; set; }
        public string UserId { get; set; }
        public string PostId { get; set; }
        public string? CommentId { get; set; }
        public UserDto User { get; set; }
        public CommentReactionSummaryDto? ReactionSummary { get; set; }
    }
}

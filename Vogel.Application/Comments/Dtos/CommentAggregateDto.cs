using Vogel.Application.CommentReactions.Dtos;
using Vogel.Application.Users.Dtos;
using Vogel.BuildingBlocks.Application.Dtos;
namespace Vogel.Application.Comments.Dtos
{
    public class CommentAggregateDto : EntityDto<string>
    {
        public string Content { get; set; }
        public string UserId { get; set; }
        public string PostId { get; set; }
        public string? CommentId { get; set; }
        public PublicUserDto User { get; set; }
        public CommentReactionSummaryDto? ReactionSummary { get; set; }
    }
}

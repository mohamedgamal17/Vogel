using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.CommentReactions.Dtos;
using Vogel.Content.Domain.Common;
namespace Vogel.Content.Application.CommentReactions.Commands.UpdateCommentReaction
{
    public class UpdateCommentReactionCommand : ICommand<CommentReactionDto>
    {
        public string ReactionId { get; set; }
        public string PostId { get; set; }
        public string CommentId { get; set; }
        public ReactionType Type { get; set; }
    }
}

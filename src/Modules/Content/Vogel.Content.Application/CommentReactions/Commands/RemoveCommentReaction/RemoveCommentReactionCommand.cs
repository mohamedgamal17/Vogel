using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
namespace Vogel.Content.Application.CommentReactions.Commands.RemoveCommentReaction
{
    [Authorize] 
    public class RemoveCommentReactionCommand : ICommand
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
        public string ReactionId { get; set; }
    }
}

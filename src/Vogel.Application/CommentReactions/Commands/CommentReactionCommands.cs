using MediatR;
using Microsoft.AspNetCore.Authorization;
using Vogel.Application.CommentReactions.Dtos;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Domain.Posts;
namespace Vogel.Application.CommentReactions.Commands
{
    
    public abstract class CommentReactionCommandBase
    {
        public string PostId { get;  set; }
        public string CommentId { get;  set; }
        public ReactionType Type { get; set; }

    }

    [Authorize]
    public class CreateCommentReactionCommand : CommentReactionCommandBase , ICommand<CommentReactionDto>
    {

    }

    [Authorize]
    public class UpdateCommentReactionCommand : CommentReactionCommandBase , ICommand<CommentReactionDto>
    {
        public string ReactionId { get; set; }

    }

    [Authorize]
    public class RemoveCommentReactionCommand : ICommand<Unit>
    {
        public string Id { get; set; }

        public string PostId { get; set; }
        public string CommentId { get; set; }
    }
}

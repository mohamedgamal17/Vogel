using MediatR;
using Microsoft.AspNetCore.Authorization;
using Vogel.Application.CommentReactions.Dtos;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Domain.Posts;
namespace Vogel.Application.CommentReactions.Commands
{
    
    public abstract class CommentReactionCommandBase
    {
        public string PostId { get; private set; }
        public string CommentId { get; private set; }
        public ReactionType Type { get; set; }

        public void SetPostId(string postId)
        {
            PostId = postId;
        }

        public void SetCommentId(string commentId)
        {
            CommentId = commentId;
        }
    }

    [Authorize]
    public class CreateCommentReactionCommand : CommentReactionCommandBase , ICommand<CommentReactionDto>
    {

    }

    [Authorize]
    public class UpdateCommentReactionCommand : CommentReactionCommandBase , ICommand<CommentReactionDto>
    {
        public string Id { get; set; }

        public void SetId(string id)
        {
            Id = id;
        }
    }

    [Authorize]
    public class RemoveCommentReactionCommand : ICommand<Unit>
    {
        public string Id { get; set; }

        public string PostId { get; set; }
        public string CommentId { get; set; }
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Vogel.Application.Comments.Dtos;
using Vogel.BuildingBlocks.Application.Requests;
namespace Vogel.Application.Comments.Commands
{
    public abstract class CommentCommandBase
    {
        public string Content { get; set; }
        public string PostId { get; protected set; }

        public void SetPostId(string postId) => PostId = PostId;
    }

    [Authorize]
    public class CreateCommentCommand : CommentCommandBase , ICommand<CommentAggregateDto>
    {
        public string? CommentId { get; set; }
    }

    [Authorize]
    public class UpdateCommentCommand : CommentCommandBase, ICommand<CommentAggregateDto>
    {
        public string  Id { get;protected  set ; }

        public void SetId(string id) => Id = Id;
     
    }

    [Authorize]
    public class RemoveCommentCommand : ICommand<Unit>
    {
        public string Id { get; set; }

        public string PostId { get; set; }


    }
}

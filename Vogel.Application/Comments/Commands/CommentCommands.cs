using MediatR;
using Vogel.Application.Comments.Dtos;
using Vogel.Application.Common.Interfaces;

namespace Vogel.Application.Comments.Commands
{
    public abstract class CommentCommandBase
    {
        public string Content { get; set; }

        public string PostId { get; set; }
    }

    public class CreateCommentCommand : CommentCommandBase , ICommand<CommentDto>
    { }

    public class UpdateCommentCommand : CommentCommandBase, ICommand<CommentDto>
    {
        public string Id { get; set; }
    }

    public class RemoveCommentCommand : ICommand<Unit>
    {
        public string Id { get; set; }

        public string PostId { get; set; }
    }
}

using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Domain.Comments;
namespace Vogel.Content.Application.Comments.Commands.RemoveComent
{
    [Authorize]
    public class RemoveCommentCommand : ICommand
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
    }

    public class RemoveCommentCommandValidator : AbstractValidator<RemoveCommentCommand>
    {
        public RemoveCommentCommandValidator()
        {
            RuleFor(x => x.PostId)
             .MaximumLength(CommentTableConsts.PostIdLength)
             .NotNull()
             .NotEmpty();

            RuleFor(x => x.CommentId)
                .MaximumLength(CommentTableConsts.IdLength)
                .NotEmpty()
                .NotNull();
        }
    }
}

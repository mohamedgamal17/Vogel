using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.Comments.Dtos;
using Vogel.Content.Domain.Comments;
namespace Vogel.Content.Application.Comments.Commands.UpdateComment
{
    [Authorize]
    public class UpdateCommentCommand : ICommand<CommentDto>
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
        public string Content { get; set; }
 
    }

    public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
    {
        public UpdateCommentCommandValidator()
        {
            RuleFor(x => x.Content)
               .MaximumLength(CommentTableConsts.ContentLength)
               .NotEmpty()
               .NotNull();

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

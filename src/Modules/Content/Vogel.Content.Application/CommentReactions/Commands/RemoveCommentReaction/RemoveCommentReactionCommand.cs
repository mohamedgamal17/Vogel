using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Domain.Comments;
namespace Vogel.Content.Application.CommentReactions.Commands.RemoveCommentReaction
{
    [Authorize] 
    public class RemoveCommentReactionCommand : ICommand
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
        public string ReactionId { get; set; }
    }

    public class RemoveCommentReactionCommandValidator : AbstractValidator<RemoveCommentReactionCommand>
    {
        public RemoveCommentReactionCommandValidator()
        {
            RuleFor(x => x.ReactionId)
                .NotEmpty()
                .NotNull()
                .MaximumLength(CommentReactionTableConsts.IdLength);


            RuleFor(x => x.PostId)
                .NotEmpty()
                .NotNull()
                .MaximumLength(CommentTableConsts.PostIdLength);


            RuleFor(x => x.CommentId)
                .NotEmpty()
                .NotNull()
                .MaximumLength(CommentReactionTableConsts.CommentIdLength);

        }
    }
}

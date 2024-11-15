using FluentValidation;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.CommentReactions.Dtos;
using Vogel.Content.Domain.Comments;
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

    public class UpdateCommentReactionCommandValidator : AbstractValidator<UpdateCommentReactionCommand>
    {
        public UpdateCommentReactionCommandValidator()
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

            RuleFor(x => x.Type).IsInEnum();
        }
    }
}

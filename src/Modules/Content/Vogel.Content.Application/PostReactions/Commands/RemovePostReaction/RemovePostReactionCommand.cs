using FluentValidation;
using MediatR;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Domain.Posts;

namespace Vogel.Content.Application.PostReactions.Commands.RemovePostReaction
{
    public class RemovePostReactionCommand : ICommand<Unit>
    {
        public string ReactionId { get; set; }
        public string PostId { get; set; }
    }

    public class RemovePostReactionCommandValidator : AbstractValidator<RemovePostReactionCommand>
    {
        public RemovePostReactionCommandValidator()
        {
            RuleFor(x => x.ReactionId)
                .MaximumLength(PostReactionTableConsts.IdLength)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.PostId)
                .MaximumLength(PostReactionTableConsts.PostIdLength)
                .NotEmpty()
                .NotNull();
        }
    }

}

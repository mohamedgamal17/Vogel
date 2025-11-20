using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Domain.Posts;

namespace Vogel.Content.Application.PostReactions.Commands.RemovePostReaction
{
    [Authorize]
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

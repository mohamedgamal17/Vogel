using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.Content.Domain.Common;
using Vogel.Content.Domain.Posts;
namespace Vogel.Content.Application.PostReactions.Commands.CreatePostReaction
{
    [Authorize]
    public class CreatePostReactionCommand : ICommand<PostReactionDto>
    {
        public string PostId { get; set; }
        public ReactionType Type { get; set; }
    }

    public class CreatePostReactionCommandValidator : AbstractValidator<CreatePostReactionCommand>
    {
        public CreatePostReactionCommandValidator()
        {
            RuleFor(x => x.PostId)
                .MaximumLength(PostReactionTableConsts.PostIdLength)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.Type).IsInEnum();
        }
    }
}

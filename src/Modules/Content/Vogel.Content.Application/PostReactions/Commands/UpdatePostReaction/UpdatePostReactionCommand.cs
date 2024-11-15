using FluentValidation;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.PostReactions.Commands.CreatePostReaction;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.Content.Domain.Common;
using Vogel.Content.Domain.Posts;
namespace Vogel.Content.Application.PostReactions.Commands.UpdatePostReaction
{
    public class UpdatePostReactionCommand : ICommand<PostReactionDto>
    {
        public string ReactionId { get; set; }
        public string PostId { get; set; }
        public ReactionType Type { get; set; }
    }

    public class UpdatePostReactionCommandValidator : AbstractValidator<UpdatePostReactionCommand>
    {
        public UpdatePostReactionCommandValidator()
        {
            RuleFor(x => x.ReactionId)
                .MaximumLength(PostReactionTableConsts.IdLength)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.PostId)
                .MaximumLength(PostReactionTableConsts.PostIdLength)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.Type).IsInEnum();

        }
    }
}

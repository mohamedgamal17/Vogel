using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.CommentReactions.Dtos;
using Vogel.Content.Domain.Comments;
using Vogel.Content.Domain.Common;
using Vogel.Content.MongoEntities.Comments;

namespace Vogel.Content.Application.CommentReactions.Commands.CreateCommentReaction
{
    [Authorize]
    public class CreateCommentReactionCommand : ICommand<CommentReactionDto>
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
        public ReactionType Type { get; set; }
    }

    public class CreateCommentReactionCommandValidator : AbstractValidator<CreateCommentReactionCommand>
    {
        public CreateCommentReactionCommandValidator()
        {
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

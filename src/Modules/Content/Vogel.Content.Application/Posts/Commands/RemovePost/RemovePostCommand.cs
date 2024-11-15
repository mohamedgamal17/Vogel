using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.Posts.Factories;
using Vogel.Content.Domain.Medias;
using Vogel.Content.Domain.Posts;
namespace Vogel.Content.Application.Posts.Commands.RemovePost
{
    [Authorize]
    public class RemovePostCommand : ICommand
    {
        public string PostId { get; set; }
    }


    public class RemovePostCommandValidator : AbstractValidator<RemovePostCommand>
    {
        public RemovePostCommandValidator()
        {
            RuleFor(x => x.PostId)
               .MaximumLength(PostTableConsts.IdLength)
               .NotEmpty()
               .NotNull();
        }
    }
}

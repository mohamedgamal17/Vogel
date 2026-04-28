using FluentValidation;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.Posts.Dtos;
using Vogel.Content.Domain.Posts;
using Microsoft.AspNetCore.Authorization;
using Vogel.MediaEngine.Shared.Services;

namespace Vogel.Content.Application.Posts.Commands.UpdatePost
{
    [Authorize]
    public class UpdatePostCommand : ICommand<PostDto>
    {
        public string PostId { get; set; }
        public string Caption { get; set; }
        public string? MediaId { get; set; }
    }

    public class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
    {
        private readonly IMediaService _mediaService;
        public UpdatePostCommandValidator(IMediaService mediaService)
        {
            _mediaService = mediaService;

            RuleFor(x => x.PostId)
                .MaximumLength(PostTableConsts.IdLength)
                .NotEmpty()
                .NotNull();

            RuleFor(x => x.Caption)
                .MaximumLength(PostTableConsts.CaptionLength)
                .NotNull()
                .NotEmpty()
                .When(x => x.Caption != null || x.MediaId == null);

            RuleFor(x => x.MediaId)
                .MaximumLength(256)
                .NotNull()
                .NotEmpty()
                .MustAsync(CheckMediaExist)
                .WithMessage((_, mediaId) => $"Media with id : ({mediaId}) , is not exist")
                .When(x => x.MediaId != null);

        }

        private async Task<bool> CheckMediaExist(string mediaId, CancellationToken cancellationToken)
        {
            var media = await _mediaService.GetMediaById(mediaId);

            return media.IsSuccess;
        }
    }
}

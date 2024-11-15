using FluentValidation;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.Posts.Dtos;
using Vogel.Content.Domain.Medias;
using Vogel.Content.Domain.Posts;
using Vogel.Content.Domain;

namespace Vogel.Content.Application.Posts.Commands.UpdatePost
{
    public class UpdatePostCommand : ICommand<PostDto>
    {
        public string PostId { get; set; }
        public string Caption { get; set; }
        public string? MediaId { get; set; }
    }

    public class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
    {
        private readonly IContentRepository<Media> _mediaRepository;
        public UpdatePostCommandValidator(IContentRepository<Media> mediaRepository)
        {
            _mediaRepository = mediaRepository;

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
            return await _mediaRepository.AnyAsync(x => x.Id == mediaId);
        }
    }
}

using FastEndpoints;
using FluentValidation;
using MimeMapping;
using Vogel.MediaEngine.Domain.Medias;

namespace Vogel.MediaEngine.Presentation.Endpoints.Medias
{
    public class CreateMediaEndpointValidator : Validator<CreateMediaRequest>
    {
        private const long MaxImageSizeInBytes = 10 * 1024 * 1024;
        private const long MaxVideoSizeInBytes = 100 * 1024 * 1024;

        public CreateMediaEndpointValidator()
        {
            RuleFor(x => x.File)
                .NotNull()
                .WithMessage("File is required.");

            RuleFor(x => x.MediaType)
                .IsInEnum();

            When(x => x.File != null, () =>
            {
                RuleFor(x => x.File.Length)
                    .GreaterThan(0)
                    .WithMessage("File cannot be empty.");

                RuleFor(x => x)
                    .Must(HaveMatchingMimeType)
                    .WithMessage("File MIME type does not match selected media type.");

                RuleFor(x => x.File.Length)
                    .LessThanOrEqualTo(MaxImageSizeInBytes)
                    .WithMessage($"Image size must be less than or equal to {MaxImageSizeInBytes / (1024 * 1024)}MB.")
                    .When(x => x.MediaType == MediaType.Image);

                RuleFor(x => x.File.Length)
                    .LessThanOrEqualTo(MaxVideoSizeInBytes)
                    .WithMessage($"Video size must be less than or equal to {MaxVideoSizeInBytes / (1024 * 1024)}MB.")
                    .When(x => x.MediaType == MediaType.Video);
            });
        }

        private static bool HaveMatchingMimeType(CreateMediaRequest request)
        {
            var mimeType = MimeUtility.GetMimeMapping(request.File.FileName);
            return request.MediaType switch
            {
                MediaType.Image => mimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase),
                MediaType.Video => mimeType.StartsWith("video/", StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }
    }
}

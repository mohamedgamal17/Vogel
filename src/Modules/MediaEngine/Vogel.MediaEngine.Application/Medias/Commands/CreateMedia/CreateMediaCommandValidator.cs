using FluentValidation;

namespace Vogel.MediaEngine.Application.Medias.Commands.CreateMedia
{
    public class CreateMediaCommandValidator : AbstractValidator<CreateMediaCommand>
    {
        private const int MaxFileNameLength = 512;

        public CreateMediaCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .MaximumLength(MaxFileNameLength);

            RuleFor(x => x.MimeType)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.MediaType)
                .IsInEnum();

            RuleFor(x => x.Content)
                .NotNull()
                .Must(HaveContent)
                .WithMessage("Media content stream must be seekable and non-empty.");
        }

        private static bool HaveContent(Stream? stream)
        {
            return stream != null && stream.CanSeek && stream.Length > 0;
        }

    }
}

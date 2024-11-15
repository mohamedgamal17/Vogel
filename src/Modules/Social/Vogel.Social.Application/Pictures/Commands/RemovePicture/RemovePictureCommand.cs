using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Social.Domain.Pictures;
namespace Vogel.Social.Application.Pictures.Commands.RemovePicture
{
    [Authorize]
    public class RemovePictureCommand : ICommand
    {
        public string Id { get; set; }
    }

    public class RemovePictureCommandValidator : AbstractValidator<RemovePictureCommand>
    {
        public RemovePictureCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .NotNull()
                .MaximumLength(PictureTableConsts.IdLength);
        }
    }
}

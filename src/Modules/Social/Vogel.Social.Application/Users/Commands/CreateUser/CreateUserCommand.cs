using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver.Linq;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Users;
using Vogel.MediaEngine.Shared.Services;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Social.Application.Users.Commands.CreateUser
{
    [Authorize]
    public class CreateUserCommand : ICommand<UserDto>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Shared.Common.Gender Gender { get; set; }
        public string? AvatarId { get; set; }
    }


    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        private readonly IMediaService _mediaService;
        public CreateUserCommandValidator(IMediaService mediaService)
        {
            _mediaService = mediaService;

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .NotNull()
                .MinimumLength(2)
                .MaximumLength(UserTableConsts.FirstNameLength);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .NotNull()
                .MinimumLength(2)
                .MaximumLength(UserTableConsts.LastNameLength);

            RuleFor(x => x.BirthDate)
                .LessThanOrEqualTo(DateTime.Now.AddYears(-12))
                .WithMessage("User age must be greater than or equal to 12 years")
                .GreaterThan(DateTime.Now.AddYears(-100))
                .WithMessage("Invalid user age");

            RuleFor(x => x.Gender).IsInEnum();

            RuleFor(x => x.AvatarId)
                .NotEmpty()
                .NotNull()
                .MaximumLength(UserTableConsts.AvatarIdLength)
                .MustAsync(CheckMediaExist)
                .WithMessage((_, mediaId) => $"Media with id : ({mediaId}) , is not exist")
                .When(x => x.AvatarId != null);
        }


        private async Task<bool> CheckMediaExist(string mediaId, CancellationToken cancellationToken = default)
        {
            var media = await _mediaService.GetMediaById(mediaId);
            return media.IsSuccess;
        }
    }
}

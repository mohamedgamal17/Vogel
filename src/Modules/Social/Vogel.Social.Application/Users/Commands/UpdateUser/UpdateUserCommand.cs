using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Repositories;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Pictures;
using Vogel.Social.Domain.Users;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Users.Commands.UpdateUser
{
    [Authorize]
    public class UpdateUserCommand : ICommand<UserDto>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Shared.Common.Gender Gender { get; set; }
        public string? AvatarId { get; set; }
    }

    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        private readonly ISocialRepository<Picture> _pictureRepository;
        public UpdateUserCommandValidator(ISocialRepository<Picture> pictureRepository)
        {
            _pictureRepository = pictureRepository;

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
                .MustAsync(CheckPictureExist)
                .WithMessage((_, pictureId) => $"Picture with id : ({pictureId}) , is not exist")
                .When(x => x.AvatarId != null);
        }


        private async Task<bool> CheckPictureExist(string pictureId, CancellationToken cancellationToken = default)
        {
            return await _pictureRepository.AnyAsync(x => x.Id == pictureId);
        }
    }
}

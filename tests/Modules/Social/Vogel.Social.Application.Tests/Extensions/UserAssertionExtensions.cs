using FluentAssertions;
using Vogel.Social.Application.Tests.Extensions;
using Vogel.Social.Application.Users.Commands.CreateUser;
using Vogel.Social.Application.Users.Commands.UpdateUser;
using Vogel.Social.Domain.Users;
using Vogel.Social.MongoEntities.Users;
using Vogel.Application.Tests.Extensions;
using Vogel.Social.Shared.Dtos;
using Vogel.Social.Domain.Pictures;
namespace Vogel.Social.Application.Tests.Extensions
{
    public static class UserAssertionExtensions
    {
        public static void AssertUser(this User user, CreateUserCommand command)
        {
            user.FirstName.Should().Be(command.FirstName);
            user.LastName.Should().Be(command.LastName);
            user.Gender.Should().Be(command.Gender);
            user.BirthDate.Should().BeSameDateAs(command.BirthDate);
            user.AvatarId.Should().Be(command.AvatarId);
        }

        public static void AssertUser(this User user , UpdateUserCommand command)
        {
            user.FirstName.Should().Be(command.FirstName);
            user.LastName.Should().Be(command.LastName);
            user.Gender.Should().Be(command.Gender);
            user.BirthDate.Should().BeSameDateAs(command.BirthDate);
            user.AvatarId.Should().Be(command.AvatarId);
        }

        public static void AssertUserMongoEntity(this UserMongoEntity user, User mongoEntity)
        {
            user.Id.Should().Be(mongoEntity.Id);
            user.FirstName.Should().Be(mongoEntity.FirstName);
            user.LastName.Should().Be(mongoEntity.LastName);
            user.AvatarId.Should().Be(mongoEntity.AvatarId);
            user.Gender.Should().Be(mongoEntity.Gender);
            user.AssertAuditingProperties(mongoEntity);
        }

        public static void AssertUserDto(this UserDto userDto, User user, Picture? picture = null)
        {

            userDto.Id.Should().Be(user.Id);
            userDto.FirstName.Should().Be(user.FirstName);
            userDto.LastName.Should().Be(user.LastName);
            userDto.AvatarId.Should().Be(user.AvatarId);
            // userDto.BirthDate.Should().BeSameDateAs(userAggregate.BirthDate);
            if (picture != null)
            {
                userDto.Avatar.Should().NotBeNull();
                userDto.Avatar!.AssertPictureDto(picture);
            }
        }
    }
}

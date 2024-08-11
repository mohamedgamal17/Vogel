using FluentAssertions;
using Vogel.Application.Users.Commands;
using Vogel.Application.Users.Dtos;
using Vogel.Domain.Medias;
using Vogel.Domain.Users;
using Vogel.MongoDb.Entities.Users;
namespace Vogel.Application.IntegrationTest.Extensions
{
    public static class UserAssertionExtensions
    {
        public static void AssertUser(this User user, UserCommandBase command)
        {
            user.FirstName.Should().Be(command.FirstName);
            user.LastName.Should().Be(command.LastName);
            user.Gender.Should().Be(command.Gender);
            user.BirthDate.Should().BeSameDateAs(command.BirthDate);
            user.AvatarId.Should().Be(command.AvatarId);
        }

        public static void AssertUserMongoEntity(this User user,  UserMongoEntity mongoEntity)
        {
            user.Id.Should().Be(mongoEntity.Id);
            user.FirstName.Should().Be(mongoEntity.FirstName);
            user.LastName.Should().Be(mongoEntity.LastName);
            user.AvatarId.Should().Be(mongoEntity.AvatarId);
            user.Gender.Should().Be((Domain.Users.Gender)mongoEntity.Gender);
            user.AssertAuditingProperties(mongoEntity);
        }

        public static void AssertUserDto(this UserDto userDto, User user, Media? media = null)
        {

            userDto.Id.Should().Be(user.Id);
            userDto.FirstName.Should().Be(user.FirstName);
            userDto.LastName.Should().Be(user.LastName);
            userDto.AvatarId.Should().Be(user.AvatarId);
            // userDto.BirthDate.Should().BeSameDateAs(userAggregate.BirthDate);
            if (media != null )
            {
                userDto.Avatar.Should().NotBeNull();
                userDto.Avatar!.AssertMediaDto(media);
            }
        }
    }
}

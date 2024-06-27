using FluentAssertions;
using Vogel.Application.Users.Commands;
using Vogel.Domain.Users;
using Vogel.MongoDb.Entities.Users;
namespace Vogel.Application.IntegrationTest.Extensions
{
    public static class UserAssertionExtensions
    {
        public static void AssertUser(this UserAggregate user, UserCommandBase command)
        {
            user.FirstName.Should().Be(command.FirstName);
            user.LastName.Should().Be(command.LastName);
            user.Gender.Should().Be(command.Gender);
            user.BirthDate.Should().BeSameDateAs(command.BirthDate);
            user.AvatarId.Should().Be(command.AvatarId);
        }

        public static void AssertUserMongoEntity(this UserAggregate user,  UserMongoEntity mongoEntity)
        {
            user.Id.Should().Be(mongoEntity.Id);
            user.FirstName.Should().Be(mongoEntity.FirstName);
            user.LastName.Should().Be(mongoEntity.LastName);
            user.AvatarId.Should().Be(mongoEntity.AvatarId);
            user.Gender.Should().Be((Domain.Users.Gender)mongoEntity.Gender);
            user.AssertAuditingProperties(mongoEntity);
        }
    }
}

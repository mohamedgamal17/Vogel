using FluentAssertions;
using Vogel.Application.Users.Commands;
using Vogel.Domain;

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
    }
}

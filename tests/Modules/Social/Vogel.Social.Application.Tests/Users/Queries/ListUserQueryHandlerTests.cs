using FluentAssertions;
using Vogel.Application.Tests.Extensions;
using Vogel.Social.Application.Users.Queries.ListUsers;

namespace Vogel.Social.Application.Tests.Users.Queries
{
    public class ListUserQueryHandlerTests : SocialTestFixture
    {
        [Test]
        public async Task Should_return_paged_list_of_users()
        {
            UserService.Login();

            var query = new ListUsersQuery
            {
                Limit = 10
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.Data.Count.Should().Be(query.Limit);
        }

        [Test]
        public async Task Should_failure_while_listing_users_when_user_is_not_authorized()
        {
            var query = new ListUsersQuery
            {
                Limit = 10
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }
    }
}

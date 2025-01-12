using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.Social.Application.Friendship.Queries.ListFriendRequest;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Users;

namespace Vogel.Social.Application.Tests.Friendship.Queries
{
    public class ListFriendRequestQueryHandlerTests : SocialTestFixture
    {
        public ISocialRepository<User> UserRepository { get; }
        public ListFriendRequestQueryHandlerTests()
        {
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
        }

        [Test]
        public async Task Should_list_current_user_friend_requests()
        {
            var currentUser = await UserRepository.AsQuerable().PickRandom();

            AuthenticationService.Login(currentUser!.Id, currentUser.FirstName + currentUser.LastName, new List<string>());

            var query = new ListFriendRequestQuery() { UserId = currentUser.Id , Limit = 50};

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.Data.All(x => x.ReciverId == currentUser.Id).Should().BeTrue();
        }

        [Test]
        public async Task Should_failure_while_listing_friend_requests_when_user_is_not_authorized()
        {
            var query = new ListFriendRequestQuery() { UserId = Guid.NewGuid().ToString() };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));

        }

    }
}

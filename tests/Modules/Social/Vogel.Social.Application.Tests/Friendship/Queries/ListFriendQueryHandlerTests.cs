using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.Social.Application.Friendship.Queries.ListFriends;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Users;
namespace Vogel.Social.Application.Tests.Friendship.Queries
{
    public class ListFriendQueryHandlerTests : SocialTestFixture
    {
        public ISocialRepository<User> UserRepository { get;  }

        public ListFriendQueryHandlerTests()
        {
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
        }

        [Test]
        public async Task Should_list_user_friends()
        {
            var currentUser = await UserRepository.AsQuerable().PickRandom();

            UserService.Login(currentUser!.Id, currentUser.FirstName + currentUser.LastName, new List<string>());

            var query = new ListFriendsQuery
            {
                UserId = currentUser.Id,
                Limit =50
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.Data.All(x => x.SourceId == currentUser.Id || x.TargetId == currentUser.Id).Should().BeTrue();
        }

        [Test]
        public async Task Should_failure_while_listing_user_friends_when_user_is_not_authorized()
        {
            var query = new ListFriendsQuery
            {
                UserId = Guid.NewGuid().ToString(),
                Limit = 50
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));

        }
    }
}

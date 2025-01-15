using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.Content.Application.Posts.Queries.ListUserPost;
using Vogel.Content.Application.Tests.Fakers;

namespace Vogel.Content.Application.Tests.Posts.Queries
{
    public class ListUserPostQueryHandlerTests : ContentTestFixture
    {
        public FakeUserFriendService UserFriendService { get; }

        public ListUserPostQueryHandlerTests()
        {
            UserFriendService = ServiceProvider.GetRequiredService<FakeUserFriendService>();
        }
 

        [Test]
        public async Task Should_list_posts_for_specific_user()
        {
            var fakeUser = UserService.PickRandomUser();
            var friend = UserFriendService.PickRandomFriend(fakeUser!.Id);

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            var query = new ListUserPostQuery
            {
                UserId = friend!.TargetId,
                Limit = 10
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.Data.Count.Should().BeGreaterThan(0);

            result.Value!.Data.All(x => x.UserId == friend.TargetId).Should().BeTrue();
        }

        [Test]
        public async Task Should_failure_while_listing_posts_for_specific_user_when_current_user_is_not_authorized()
        {
            var query = new ListUserPostQuery
            {
                UserId = Guid.NewGuid().ToString(),
                Limit=  10
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }
    }
}

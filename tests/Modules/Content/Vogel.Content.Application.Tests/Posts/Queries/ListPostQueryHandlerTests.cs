using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.Content.Application.Posts.Queries.ListPost;
using Vogel.Content.Application.Tests.Fakers;
namespace Vogel.Content.Application.Tests.Posts.Queries
{
    public class ListPostQueryHandlerTests : ContentTestFixture
    {
        public FakeUserFriendService UserFriendService { get; }

        public ListPostQueryHandlerTests()
        {
            UserFriendService = ServiceProvider.GetRequiredService<FakeUserFriendService>();
        }
        [Test]
        public async Task Should_list_user_and_user_friends_latest_posts()
        {
            var fakeUser = UserService.PickRandomUser();
            var userFriends = UserFriendService.GetUserFriends(fakeUser!.Id);
            List<string> expectedIds = userFriends.Select(x => x.TargetId).ToList();
            expectedIds.Add(fakeUser.Id);

            AuthenticationService.Login(fakeUser.Id, fakeUser.FirstName + fakeUser.LastName, new List<string>());

            var query = new ListPostQuery
            {
                Limit = 150
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.Data.Count.Should().Be(12);

            result.Value.Data.All(x => expectedIds.Contains(x.UserId)).Should().BeTrue();
        }

        [Test]
        public async Task Should_failure_while_listing_user_and_user_friends_posts_when_user_is_not_authorized()
        {
            var query = new ListPostQuery
            {
                Limit = 10
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

    }
}

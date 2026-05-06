using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.Social.Application.Tests.Fakers;
using Vogel.Social.Application.Friendship.Queries.ListFriendRequest;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Users;

namespace Vogel.Social.Application.Tests.Friendship.Queries
{
    public class ListFriendRequestQueryHandlerTests : SocialTestFixture
    {
        public ISocialRepository<User> UserRepository { get; }
        public FakeMediaService FakeMediaService { get; }
        public ListFriendRequestQueryHandlerTests()
        {
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
            FakeMediaService = ServiceProvider.GetRequiredService<FakeMediaService>();
        }

        [Test]
        public async Task Should_list_current_user_friend_requests()
        {
            var currentUser = await UserRepository.AsQuerable().PickRandom();
            var avatar = FakeMediaService.AddMedia(currentUser!.Id);
            var cover = FakeMediaService.AddMedia(currentUser.Id);
            currentUser.AvatarId = avatar.Id;
            currentUser.CoverId = cover.Id;
            await UserRepository.UpdateAsync(currentUser);

            AuthenticationService.Login(currentUser!.Id, currentUser.FirstName + currentUser.LastName, new List<string>());

            var query = new ListFriendRequestQuery() { UserId = currentUser.Id , Limit = 50};

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.Data.All(x => x.ReciverId == currentUser.Id).Should().BeTrue();
            var receiver = result.Value.Data.FirstOrDefault(x => x.ReciverId == currentUser.Id && x.Reciver != null)?.Reciver;
            receiver.Should().NotBeNull();
            receiver!.AvatarId.Should().Be(avatar.Id);
            receiver.Avatar.Should().NotBeNull();
            receiver.Avatar!.Id.Should().Be(avatar.Id);
            receiver.CoverId.Should().Be(cover.Id);
            receiver.Cover.Should().NotBeNull();
            receiver.Cover!.Id.Should().Be(cover.Id);
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

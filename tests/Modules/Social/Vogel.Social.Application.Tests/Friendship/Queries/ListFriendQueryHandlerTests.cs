using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.Social.Application.Tests.Fakers;
using Vogel.Social.Application.Friendship.Queries.ListFriends;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Users;
namespace Vogel.Social.Application.Tests.Friendship.Queries
{
    public class ListFriendQueryHandlerTests : SocialTestFixture
    {
        public ISocialRepository<User> UserRepository { get;  }
        public FakeMediaService FakeMediaService { get; }

        public ListFriendQueryHandlerTests()
        {
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
            FakeMediaService = ServiceProvider.GetRequiredService<FakeMediaService>();
        }

        [Test]
        public async Task Should_list_user_friends()
        {
            var currentUser = await UserRepository.AsQuerable().PickRandom();
            var avatar = FakeMediaService.AddMedia(currentUser!.Id);
            var cover = FakeMediaService.AddMedia(currentUser.Id);
            currentUser.AvatarId = avatar.Id;
            currentUser.CoverId = cover.Id;
            await UserRepository.UpdateAsync(currentUser);

            AuthenticationService.Login(currentUser!.Id, currentUser.FirstName + currentUser.LastName, new List<string>());

            var query = new ListFriendsQuery
            {
                UserId = currentUser.Id,
                Limit =50
            };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.Data.All(x => x.SourceId == currentUser.Id || x.TargetId == currentUser.Id).Should().BeTrue();

            var fromSourceSide = result.Value.Data.FirstOrDefault(x => x.SourceId == currentUser.Id && x.Source != null);
            var fromTargetSide = result.Value.Data.FirstOrDefault(x => x.TargetId == currentUser.Id && x.Target != null);
            var currentUserDto = fromSourceSide?.Source ?? fromTargetSide?.Target;
            currentUserDto.Should().NotBeNull();
            currentUserDto!.AvatarId.Should().Be(avatar.Id);
            currentUserDto.Avatar.Should().NotBeNull();
            currentUserDto.Avatar!.Id.Should().Be(avatar.Id);
            currentUserDto.CoverId.Should().Be(cover.Id);
            currentUserDto.Cover.Should().NotBeNull();
            currentUserDto.Cover!.Id.Should().Be(cover.Id);
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

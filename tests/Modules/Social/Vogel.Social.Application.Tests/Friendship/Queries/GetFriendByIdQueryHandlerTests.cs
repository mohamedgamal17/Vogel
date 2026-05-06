using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Social.Application.Friendship.Queries.GetFriendById;
using Vogel.Social.Application.Tests.Extensions;
using Vogel.Social.Application.Tests.Fakers;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.Domain.Users;
namespace Vogel.Social.Application.Tests.Friendship.Queries
{
    public class GetFriendByIdQueryHandlerTests : SocialTestFixture
    {
        public ISocialRepository<User> UserRepository { get; }
        public ISocialRepository<Friend> FriendRepository { get;  }
        public FakeMediaService FakeMediaService { get; }
        public GetFriendByIdQueryHandlerTests()
        {
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
            FriendRepository = ServiceProvider.GetRequiredService<ISocialRepository<Friend>>();
            FakeMediaService = ServiceProvider.GetRequiredService<FakeMediaService>();
        }

        [Test]
        public async Task Should_get_friend_by_id()
        {
            var currentUser = await UserRepository.AsQuerable().PickRandom();

            var friend = await FriendRepository.AsQuerable().Where(x => x.SourceId == currentUser!.Id).PickRandom();

            var friendUser = await UserRepository.FindByIdAsync(friend!.TargetId);
            var currentUserAvatar = FakeMediaService.AddMedia(currentUser!.Id);
            var currentUserCover = FakeMediaService.AddMedia(currentUser.Id);
            currentUser.AvatarId = currentUserAvatar.Id;
            currentUser.CoverId = currentUserCover.Id;
            await UserRepository.UpdateAsync(currentUser);

            var friendAvatar = FakeMediaService.AddMedia(friendUser!.Id);
            var friendCover = FakeMediaService.AddMedia(friendUser.Id);
            friendUser.AvatarId = friendAvatar.Id;
            friendUser.CoverId = friendCover.Id;
            await UserRepository.UpdateAsync(friendUser);

            AuthenticationService.Login(currentUser!.Id, currentUser.FirstName + currentUser.LastName, new List<string>());

            var query = new GetFriendByIdQuery { FriendId = friend.Id };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.AssertFriendDto(friend, currentUser, friendUser);
            result.Value.Source.Should().NotBeNull();
            result.Value.Source!.AvatarId.Should().Be(currentUserAvatar.Id);
            result.Value.Source.Avatar.Should().NotBeNull();
            result.Value.Source.Avatar!.Id.Should().Be(currentUserAvatar.Id);
            result.Value.Source.CoverId.Should().Be(currentUserCover.Id);
            result.Value.Source.Cover.Should().NotBeNull();
            result.Value.Source.Cover!.Id.Should().Be(currentUserCover.Id);

            result.Value.Target.Should().NotBeNull();
            result.Value.Target!.AvatarId.Should().Be(friendAvatar.Id);
            result.Value.Target.Avatar.Should().NotBeNull();
            result.Value.Target.Avatar!.Id.Should().Be(friendAvatar.Id);
            result.Value.Target.CoverId.Should().Be(friendCover.Id);
            result.Value.Target.Cover.Should().NotBeNull();
            result.Value.Target.Cover!.Id.Should().Be(friendCover.Id);
        }

        [Test]
        public async Task Should_failure_while_getting_friend_by_id_when_user_is_not_source_or_target()
        {
            var currentUser = await UserRepository.AsQuerable().PickRandom();

            var friend = await FriendRepository.AsQuerable().Where(x => x.SourceId != currentUser!.Id && x.TargetId != currentUser.Id).PickRandom();

            AuthenticationService.Login(currentUser!.Id, currentUser.FirstName + currentUser.LastName, new List<string>());

            var query = new GetFriendByIdQuery { FriendId = friend!.Id };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }

        [Test]
        public async Task Should_failure_while_getting_friend_by_id_when_id_is_not_exist()
        {
            AuthenticationService.Login();

            var query = new GetFriendByIdQuery { FriendId = Guid.NewGuid().ToString() };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_failure_while_getting_friend_by_id_when_user_is_not_authorized()
        {
            var query = new GetFriendByIdQuery { FriendId = Guid.NewGuid().ToString() };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }
    }
}

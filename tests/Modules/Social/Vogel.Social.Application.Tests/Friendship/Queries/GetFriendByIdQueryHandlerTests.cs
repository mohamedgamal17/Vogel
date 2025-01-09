using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Social.Application.Friendship.Queries.GetFriendById;
using Vogel.Social.Application.Tests.Extensions;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.Domain.Users;
namespace Vogel.Social.Application.Tests.Friendship.Queries
{
    public class GetFriendByIdQueryHandlerTests : SocialTestFixture
    {
        public ISocialRepository<User> UserRepository { get; }
        public ISocialRepository<Friend> FriendRepository { get;  }
        public GetFriendByIdQueryHandlerTests()
        {
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
            FriendRepository = ServiceProvider.GetRequiredService<ISocialRepository<Friend>>();
        }

        [Test]
        public async Task Should_get_friend_by_id()
        {
            var currentUser = await UserRepository.AsQuerable().PickRandom();

            var friend = await FriendRepository.AsQuerable().Where(x => x.SourceId == currentUser!.Id).PickRandom();

            var friendUser = await UserRepository.FindByIdAsync(friend!.TargetId);

            UserService.Login(currentUser!.Id, currentUser.FirstName + currentUser.LastName, new List<string>());

            var query = new GetFriendByIdQuery { FriendId = friend.Id };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.AssertFriendDto(friend, currentUser, friendUser);
        }

        [Test]
        public async Task Should_failure_while_getting_friend_by_id_when_user_is_not_source_or_target()
        {
            var currentUser = await UserRepository.AsQuerable().PickRandom();

            var friend = await FriendRepository.AsQuerable().Where(x => x.SourceId != currentUser!.Id && x.TargetId != currentUser.Id).PickRandom();

            UserService.Login(currentUser!.Id, currentUser.FirstName + currentUser.LastName, new List<string>());

            var query = new GetFriendByIdQuery { FriendId = friend!.Id };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }

        [Test]
        public async Task Should_failure_while_getting_friend_by_id_when_id_is_not_exist()
        {
            UserService.Login();

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

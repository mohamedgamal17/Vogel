using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Social.Application.Friendship.Queries.GetFriendRequestById;
using Vogel.Social.Application.Tests.Extensions;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.Domain.Users;

namespace Vogel.Social.Application.Tests.Friendship.Queries
{
    public class GetFriendRequestByIdQueryHandlerTests  : SocialTestFixture
    {
        public ISocialRepository<FriendRequest> FriendRequestRepository { get; }
        public ISocialRepository<User> UserRepository { get; }
        public GetFriendRequestByIdQueryHandlerTests()
        {
            FriendRequestRepository = ServiceProvider.GetRequiredService<ISocialRepository<FriendRequest>>();
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
        }

        [Test]
        public async Task Should_get_user_friend_request_by_id()
        {
            var currentUser = await UserRepository.AsQuerable().PickRandom();

            var friendRequest = await FriendRequestRepository.AsQuerable().Where(x => x.ReciverId == currentUser!.Id).PickRandom();

            var senderUser = await UserRepository.FindByIdAsync(friendRequest!.Id);

            AuthenticationService.Login(currentUser!.Id, currentUser.FirstName + currentUser.LastName, new List<string>());

            var query = new GetFriendRequestByIdQuery { FriendRequestId = friendRequest!.Id };

            var result = await Mediator.Send(query);

            result.ShouldBeSuccess();

            result.Value!.AssertFriendRequestDto(friendRequest, senderUser);
        }

        [Test]
        public async Task Should_failure_while_getting_user_friend_request_while_friend_request_id_is_not_exist()
        {
            var currentUser = await UserRepository.AsQuerable().PickRandom();

            AuthenticationService.Login(currentUser!.Id, currentUser.FirstName + currentUser.LastName, new List<string>());

            var query = new GetFriendRequestByIdQuery { FriendRequestId = Guid.NewGuid().ToString() };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(EntityNotFoundException));

        }


        [Test]
        public async Task Should_failure_while_getting_user_friend_request_by_id_while_user_is_not_authorized()
        {
            var query = new GetFriendRequestByIdQuery
            {
                FriendRequestId = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(query);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));

        }
    }
}

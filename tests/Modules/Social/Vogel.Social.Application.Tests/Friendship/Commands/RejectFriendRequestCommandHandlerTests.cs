using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Social.Application.Friendship.Commands.RejectFriendRequest;
using Vogel.Social.Application.Tests.Extensions;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.Domain.Users;
using Vogel.Social.MongoEntities.Friendship;
using Vogel.Social.Shared.Common;

namespace Vogel.Social.Application.Tests.Friendship.Commands
{
    public class RejectFriendRequestCommandHandlerTests : SocialTestFixture
    {
        public ISocialRepository<FriendRequest> FriendRequestRepository { get; }
        public FriendRequestMongoRepository FriendRequestMongoRepository { get; }
        public ISocialRepository<User> UserRepository { get; }

        public RejectFriendRequestCommandHandlerTests()
        {
            FriendRequestRepository = ServiceProvider.GetRequiredService<ISocialRepository<FriendRequest>>();
            FriendRequestMongoRepository = ServiceProvider.GetRequiredService<FriendRequestMongoRepository>();
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
        }

        [Test]
        public async Task Should_reciver_reject_friend_request()
        {
            UserService.Login();

            string userId = UserService.GetCurrentUser()!.Id;
            var reciver = await CreateFakeUser(userId);
            var sender = await CreateFakeUser();

            var fakeFriendRequest = await CreateFakeFriendRequest(sender, reciver);

            var command = new RejectFriendRequestCommand { FriendRequestId = fakeFriendRequest.Id };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var friendRequest = await FriendRequestRepository.FindByIdAsync(result.Value!.Id);

            friendRequest!.State.Should().Be(FriendRequestState.Rejected);

            var monogEntity = await FriendRequestMongoRepository.FindByIdAsync(friendRequest!.Id);

            monogEntity.Should().NotBeNull();

            monogEntity!.AssertFriendRequestMongoEntity(friendRequest);

            result.Value.AssertFriendRequestDto(friendRequest, sender, reciver);
        }

        [Test]
        public async Task Should_failure_when_rejecting_friend_request_while_user_is_not_authorized()
        {
            var sender = await CreateFakeUser();
            var reciver = await CreateFakeUser();

            var fakeFriendRequest = await CreateFakeFriendRequest(sender, reciver);

            var command = new RejectFriendRequestCommand { FriendRequestId = fakeFriendRequest.Id };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));

        }

        [Test]
        public async Task Should_failure_when_rejecting_friend_request_while_user_is_not_the_real_reciver()
        {
            UserService.Login();
            string userId = UserService.GetCurrentUser()!.Id;
            var sender = await CreateFakeUser(userId);
            var reciver = await CreateFakeUser();

            var fakeFriendRequest = await CreateFakeFriendRequest(sender, reciver);

            var command = new RejectFriendRequestCommand { FriendRequestId = fakeFriendRequest.Id };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }

        private async Task<User> CreateFakeUser(string? userId = null)
        {
            Faker faker = new Faker();

            var user = new User(userId ?? Guid.NewGuid().ToString())
            {
                FirstName = faker.Person.FirstName,
                LastName = faker.Person.LastName,
                BirthDate = DateTime.Now.AddYears(-18),
                Gender = Gender.Male

            };

            return await UserRepository.InsertAsync(user);
        }

        private async Task<FriendRequest> CreateFakeFriendRequest(User sender, User reciver)
        {

            var reqeust = new FriendRequest
            {
                SenderId = sender.Id,
                ReciverId = reciver.Id,
            };

            return await FriendRequestRepository.InsertAsync(reqeust);
        }
    }

}

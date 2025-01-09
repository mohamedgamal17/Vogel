using Microsoft.Extensions.DependencyInjection;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.Domain.Users;
using Vogel.Social.Domain;
using Vogel.Social.MongoEntities.Friendship;
using Bogus;
using Vogel.Social.Shared.Common;
using Vogel.Social.Application.Friendship.Commands.CancelFriendRequest;
using Vogel.Application.Tests.Extensions;
using FluentAssertions;
using Vogel.Social.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;

namespace Vogel.Social.Application.Tests.Friendship.Commands
{
    public class CancelFriendRequestCommandHandlerTests : SocialTestFixture
    {
        public ISocialRepository<FriendRequest> FriendRequestRepository { get; }
        public FriendRequestMongoRepository FriendRequestMongoRepository { get; }
        public ISocialRepository<User> UserRepository { get; }
        public ISocialRepository<Friend> FriendRepository { get; }
        public FriendMongoRepository FriendMongoRepository { get; }

        public CancelFriendRequestCommandHandlerTests()
        {
            FriendRequestRepository = ServiceProvider.GetRequiredService<ISocialRepository<FriendRequest>>();
            FriendRequestMongoRepository = ServiceProvider.GetRequiredService<FriendRequestMongoRepository>();
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
            FriendRepository = ServiceProvider.GetRequiredService<ISocialRepository<Friend>>();
            FriendMongoRepository = ServiceProvider.GetRequiredService<FriendMongoRepository>();
        }
        [Test]
        public async Task Should_sender_cancel_friend_request()
        {
            UserService.Login();

            string userId = UserService.GetCurrentUser()!.Id;

            var sender = await CreateFakeUser(userId);
            var reciver = await CreateFakeUser();

            var fakeFriendRequest = await CreateFakeFriendRequest(sender, reciver);

            var command = new CancelFriendRequestCommand { FriendRequestId = fakeFriendRequest.Id };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var friendRequest = await FriendRequestRepository.FindByIdAsync(result.Value!.Id);

            friendRequest!.State.Should().Be(FriendRequestState.Cancelled);

            var monogEntity = await FriendRequestMongoRepository.FindByIdAsync(friendRequest!.Id);

            monogEntity.Should().NotBeNull();

            monogEntity!.AssertFriendRequestMongoEntity(friendRequest);

            result.Value.AssertFriendRequestDto(friendRequest, sender, reciver);
        }

        [Test]
        public async Task Should_failure_when_cancelling_friend_request_when_user_is_not_authorized()
        {
            var sender = await CreateFakeUser();

            var reciver = await CreateFakeUser();

            var fakeFriendRequest = await CreateFakeFriendRequest(sender, reciver);

            var command = new CancelFriendRequestCommand { FriendRequestId = fakeFriendRequest.Id };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));

        }


        [Test]
        public async Task Should_failure_when_cancelling_friend_request_when_user_is_not_the_real_sender()
        {
            UserService.Login();

            string userId = UserService.GetCurrentUser()!.Id;

            var sender = await CreateFakeUser();

            var reciver = await CreateFakeUser(userId);

            var fakeFriendRequest = await CreateFakeFriendRequest(sender, reciver);

            var command = new CancelFriendRequestCommand { FriendRequestId = fakeFriendRequest.Id };

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

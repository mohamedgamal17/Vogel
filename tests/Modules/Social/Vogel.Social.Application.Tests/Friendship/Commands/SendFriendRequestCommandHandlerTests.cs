using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Social.Application.Friendship.Commands.SendFriendRequest;
using Vogel.Social.Application.Tests.Extensions;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.Domain.Users;
using Vogel.Social.MongoEntities.Friendship;
using Vogel.Social.Shared.Common;
namespace Vogel.Social.Application.Tests.Friendship.Commands
{
    public class SendFriendRequestCommandHandlerTests : SocialTestFixture
    {
        public ISocialRepository<FriendRequest> FriendRequestRepository { get; }
        public FriendRequestMongoRepository FriendRequestMongoRepository { get; }
        public ISocialRepository<User> UserRepository { get; }

        public SendFriendRequestCommandHandlerTests()
        {
            FriendRequestRepository = ServiceProvider.GetRequiredService<ISocialRepository<FriendRequest>>();
            FriendRequestMongoRepository = ServiceProvider.GetRequiredService<FriendRequestMongoRepository>();
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
        }

        [Test]
        public async Task Should_create_friend_request()
        {
            UserService.Login();

            string userId = UserService.GetCurrentUser()!.Id;

            var currentUser = await CreateFakeUser(userId);

            var targetUser = await CreateFakeUser();

            var command = new SendFriendRequestCommand
            {
                ReciverId = targetUser.Id
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var friendRequest = await FriendRequestRepository.FindByIdAsync(result.Value!.Id);

            friendRequest.Should().NotBeNull();

            friendRequest!.AssertSendFriendRequestCommand(command, userId);

            var monogEntity = await FriendRequestMongoRepository.FindByIdAsync(friendRequest!.Id);

            monogEntity.Should().NotBeNull();

            monogEntity!.AssertFriendRequestMongoEntity(friendRequest);

            result.Value.AssertFriendRequestDto(friendRequest, currentUser, targetUser);
        }

        [Test]
        public async Task Should_failure_when_sending_friend_request_while_user_is_not_authorized()
        {
            var fakeUser = await CreateFakeUser();

            var command = new SendFriendRequestCommand
            {
                ReciverId = fakeUser.Id
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        [Test]
        public async Task Should_failure_when_sending_friend_request_while_there_is_already_pending_request()
        {
            UserService.Login();

            string userId = UserService.GetCurrentUser()!.Id;

            var currentUser = await CreateFakeUser(userId);
            var targetUser = await CreateFakeUser();

            var fakeRequest = await CreateFakeFriendRequest(currentUser, targetUser);

            var command = new SendFriendRequestCommand
            {
                ReciverId = targetUser.Id
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(BusinessLogicException));
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

using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Friendship.Commands;
using Vogel.Application.IntegrationTest.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Domain.Friendship;
using Vogel.Domain.Users;
using Vogel.MongoDb.Entities.Friendship;
using Vogel.MongoDb.Entities.Users;
using static Vogel.Application.IntegrationTest.Testing;
namespace Vogel.Application.IntegrationTest.Friendship
{
    public class FriendshipCommandHanderTests : BaseTestFixture
    {
        public FriendRequestMongoRepository FriendRequestMongoRepository { get; set; }

        public FriendMongoRepository FriendMongoRepository { get; set; }

        public FriendshipCommandHanderTests()
        {
            FriendRequestMongoRepository = Testing.ServiceProvider.GetRequiredService<FriendRequestMongoRepository>();
            FriendMongoRepository = Testing.ServiceProvider.GetRequiredService<FriendMongoRepository>();
        }

        [Test]
        public async Task Should_create_friend_request()
        {
            await RunAsUserAsync();

            var fakeUser = await CreateFakeUser();

            var command = new SendFriendRequestCommand
            {
                ReciverId = fakeUser.Id
            };

            var result = await SendAsync(command);

            result.ShouldBeSuccess();

            var friendRequest = await FindByIdAsync<FriendRequest>(result.Value!.Id);

            friendRequest.Should().NotBeNull();

            friendRequest!.AssertSendFriendRequestCommand(command);

            var monogEntity = await FriendRequestMongoRepository.FindByIdAsync(friendRequest!.Id);

            monogEntity.Should().NotBeNull();

            monogEntity!.AssertFriendRequestMongoEntity(friendRequest);

            result.Value.AssertFriendRequestDto(friendRequest, CurrentUserProfile, fakeUser);
        }

        [Test]
        public async Task Should_failure_when_sending_friend_request_while_user_is_not_authorized()
        {
            RemoveCurrentUser();

            var fakeUser = await CreateFakeUser();

            var command = new SendFriendRequestCommand
            {
                ReciverId = fakeUser.Id
            };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        [Test]
        public async Task Should_failure_when_sending_friend_request_while_there_is_already_pending_request()
        {
            await RunAsUserAsync();
            var fakeSender = CurrentUserProfile!;
            var fakeReciver = await CreateFakeUser();

            var fakeRequest = await CreateFakeFriendRequest(fakeSender, fakeReciver);

            var command = new SendFriendRequestCommand
            {
                ReciverId = fakeReciver.Id
            };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(BusinessLogicException));
        }

        [Test]
        public async Task Should_accept_friend_request_and_create_new_friend()
        {
            RemoveCurrentUser();

            await RunAsUserAsync();
            var sender = await CreateFakeUser();
            var reciver = CurrentUserProfile!;

            var fakeFriendRequest = await CreateFakeFriendRequest(sender, reciver);

            var command = new AcceptFriendRequestCommand
            {
                FriendRequestId = fakeFriendRequest.Id
            };

            var result = await SendAsync(command);

            result.ShouldBeSuccess();

            result.Value.Should().NotBeNull();

            var friendRequest = await FindByIdAsync<FriendRequest>(fakeFriendRequest.Id);

            friendRequest!.State.Should().Be(Domain.Friendship.FriendRequestState.Accepted);

            var friend = await FindByIdAsync<Friend>(result.Value!.Id);

            friend.Should().NotBeNull();

            friend!.AssertFriend(sender.Id, reciver.Id);

            var friendMongoEntity = await FriendMongoRepository.FindByIdAsync(friend!.Id);

            friendMongoEntity.Should().NotBeNull();

            friendMongoEntity!.AssertFriendMongoEntity(friend);

            result.Value.AssertFriendDto(friend,sender, reciver);
        }

        [Test]
        public async Task Should_failure_when_accepting_friend_request_while_user_is_not_authorized()
        {
            RemoveCurrentUser();

            var sender = await CreateFakeUser();
            var reciver = await CreateFakeUser();

            var fakeFriendRequest = await CreateFakeFriendRequest(sender, reciver);


            var command = new AcceptFriendRequestCommand
            {
                FriendRequestId = fakeFriendRequest.Id
            };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        [Test]
        public async Task Should_failure_when_accepting_friend_request_while_user_is_not_the_reciver_of_request()
        {
            await RunAsUserAsync();

            var sender = CurrentUserProfile!;
            var reciver = await CreateFakeUser();

            var fakeFriendRequest = await CreateFakeFriendRequest(sender, reciver);

            var command = new AcceptFriendRequestCommand
            {
                FriendRequestId = fakeFriendRequest.Id
            };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }

        [Test]
        public async Task Should_sender_cancel_friend_request()
        {
            await RunAsUserAsync();
            var sender = CurrentUserProfile!;
            var reciver = await CreateFakeUser();

            var fakeFriendRequest = await CreateFakeFriendRequest(sender, reciver);

            var command = new CancelFriendRequestCommand { FriendRequestId = fakeFriendRequest.Id };

            var result = await SendAsync(command);

            result.ShouldBeSuccess();

            var friendRequest = await FindByIdAsync<FriendRequest>(result.Value!.Id);

            friendRequest!.State.Should().Be(Domain.Friendship.FriendRequestState.Cancelled);

            var monogEntity = await FriendRequestMongoRepository.FindByIdAsync(friendRequest!.Id);

            monogEntity.Should().NotBeNull();

            monogEntity!.AssertFriendRequestMongoEntity(friendRequest);

            result.Value.AssertFriendRequestDto(friendRequest, CurrentUserProfile, reciver);
        }

        [Test]
        public async Task Should_failure_when_cancelling_friend_request_when_user_is_not_authorized()
        {
            RemoveCurrentUser();

            var sender = await CreateFakeUser();

            var reciver = await CreateFakeUser();

            var fakeFriendRequest = await CreateFakeFriendRequest(sender, reciver);

            var command = new CancelFriendRequestCommand { FriendRequestId = fakeFriendRequest.Id };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));

        }


        [Test]
        public async Task Should_failure_when_cancelling_friend_request_when_user_is_not_the_real_sender()
        {
            await RunAsUserAsync();

            var sender = await CreateFakeUser();

            var reciver = CurrentUserProfile!;


            var fakeFriendRequest = await CreateFakeFriendRequest(sender, reciver);

            var command = new CancelFriendRequestCommand { FriendRequestId = fakeFriendRequest.Id };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }



        [Test]
        public async Task Should_reciver_reject_friend_request()
        {
            await RunAsUserAsync();

            var sender = await CreateFakeUser()!;

            var reciver = CurrentUserProfile!;


            var fakeFriendRequest = await CreateFakeFriendRequest(sender, reciver);

            var command = new RejectFriendRequestCommand { FriendRequestId = fakeFriendRequest.Id };

            var result = await SendAsync(command);

            result.ShouldBeSuccess();

            var friendRequest = await FindByIdAsync<FriendRequest>(result.Value!.Id);

            friendRequest!.State.Should().Be(Domain.Friendship.FriendRequestState.Rejected);

            var monogEntity = await FriendRequestMongoRepository.FindByIdAsync(friendRequest!.Id);

            monogEntity.Should().NotBeNull();

            monogEntity!.AssertFriendRequestMongoEntity(friendRequest);

            result.Value.AssertFriendRequestDto(friendRequest, sender, reciver);

        }

        [Test]
        public async Task Should_failure_when_rejecting_friend_request_while_user_is_not_authorized()
        {
            RemoveCurrentUser();

            var sender = await CreateFakeUser();
            var reciver = await CreateFakeUser();

            var fakeFriendRequest = await CreateFakeFriendRequest(sender, reciver);

            var command = new RejectFriendRequestCommand { FriendRequestId = fakeFriendRequest.Id };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));

        }

        [Test]
        public async Task Should_failure_when_rejecting_friend_request_while_user_is_not_the_real_reciver()
        {
            await RunAsUserAsync();

            var sender = CurrentUserProfile!;
            var reciver = await CreateFakeUser();

            var fakeFriendRequest = await CreateFakeFriendRequest(sender, reciver);

            var command = new RejectFriendRequestCommand { FriendRequestId = fakeFriendRequest.Id };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }

        [Test]
        public async Task Should_remove_friend()
        {
            await RunAsUserAsync();

            var soruce = CurrentUserProfile!;

            var reciver = await CreateFakeUser();

            var fakeFriend = await CreateFakeFriend(soruce, reciver);

            var command = new RemoveFriendCommand { Id = fakeFriend.Id };

            var result = await SendAsync(command);

            result.ShouldBeSuccess();

            var friend = await FindByIdAsync<Friend>(fakeFriend.Id);

            friend.Should().BeNull();

            var mongoEntity =await FriendMongoRepository.FindByIdAsync(fakeFriend.Id);

            mongoEntity.Should().BeNull();
        }

        [Test]
        public async Task Should_failure_when_removing_friend_while_user_is_not_authorized()
        {
            RemoveCurrentUser();

            var source = await CreateFakeUser();
            var target = await CreateFakeUser();

            var fakeFriend = await CreateFakeFriend(source, target);

            var command = new RemoveFriendCommand { Id = fakeFriend.Id };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        [Test]
        public async Task Should_failure_when_removing_friend_while_user_is_not_the_soruce_or_target()
        {
            await RunAsUserAsync();

            var source = await CreateFakeUser();
            var target = await CreateFakeUser();

            var fakeFriend = await CreateFakeFriend(source, target);

            var command = new RemoveFriendCommand { Id = fakeFriend.Id };

            var result = await SendAsync(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException)); 
        }


        private async Task<User> CreateFakeUser()
        {
            Faker faker = new Faker();

            var user = new User
            {
                FirstName = faker.Person.FirstName,
                LastName = faker.Person.LastName,
                BirthDate = DateTime.Now.AddYears(-18),
                Gender = Domain.Users.Gender.Male

            };

            return await InsertAsync(user);
        }


        private async Task<FriendRequest> CreateFakeFriendRequest(User sender , User reciver)
        {

            var reqeust = new FriendRequest
            {
                SenderId = sender.Id,
                ReciverId = reciver.Id,
            };

            return await InsertAsync(reqeust);
        }


        private async Task<Friend> CreateFakeFriend(User source , User target)
        {
            var friend = new Friend
            {
                SourceId = source.Id,
                TargetId = target.Id,

            };


            return await InsertAsync(friend);
        }

    }
}

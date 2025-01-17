﻿using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Social.Application.Friendship.Commands.AcceptFriendRequest;
using Vogel.Social.Application.Tests.Extensions;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.Domain.Users;
using Vogel.Social.MongoEntities.Friendship;
using Vogel.Social.Shared.Common;

namespace Vogel.Social.Application.Tests.Friendship.Commands
{
    public class AcceptFriendRequestCommandHandlerTests : SocialTestFixture
    {
        public ISocialRepository<FriendRequest> FriendRequestRepository { get; }
        public FriendRequestMongoRepository FriendRequestMongoRepository { get; }
        public ISocialRepository<User> UserRepository { get; }
        public ISocialRepository<Friend> FriendRepository { get; }
        public FriendMongoRepository FriendMongoRepository { get; }

        public AcceptFriendRequestCommandHandlerTests()
        {
            FriendRequestRepository = ServiceProvider.GetRequiredService<ISocialRepository<FriendRequest>>();
            FriendRequestMongoRepository = ServiceProvider.GetRequiredService<FriendRequestMongoRepository>();
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
            FriendRepository = ServiceProvider.GetRequiredService<ISocialRepository<Friend>>();
            FriendMongoRepository = ServiceProvider.GetRequiredService<FriendMongoRepository>();
        }

        [Test]
        public async Task Should_accept_friend_request_and_create_new_friend()
        {
            AuthenticationService.Login();

            string userId = AuthenticationService.GetCurrentUser()!.Id;

            var currentUser = await CreateFakeUser(userId);
            var senderUser = await CreateFakeUser()!;

            var fakeFriendRequest = await CreateFakeFriendRequest(senderUser, currentUser);

            var command = new AcceptFriendRequestCommand
            {
                FriendRequestId = fakeFriendRequest.Id
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            result.Value.Should().NotBeNull();

            var friendRequest = await FriendRequestRepository.FindByIdAsync(fakeFriendRequest.Id);

            friendRequest!.State.Should().Be(FriendRequestState.Accepted);

            var friend = await FriendRepository.FindByIdAsync(result.Value!.Id);

            friend.Should().NotBeNull();

            friend!.AssertFriend(senderUser.Id, currentUser.Id);

            var friendMongoEntity = await FriendMongoRepository.FindByIdAsync(friend!.Id);

            friendMongoEntity.Should().NotBeNull();

            friendMongoEntity!.AssertFriendMongoEntity(friend);

            result.Value.AssertFriendDto(friend, senderUser, currentUser);
        }


        [Test]
        public async Task Should_failure_when_accepting_friend_request_while_user_is_not_authorized()
        {

            var sender = await CreateFakeUser();
            var reciver = await CreateFakeUser();

            var fakeFriendRequest = await CreateFakeFriendRequest(sender, reciver);


            var command = new AcceptFriendRequestCommand
            {
                FriendRequestId = fakeFriendRequest.Id
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }


        [Test]
        public async Task Should_failure_when_accepting_friend_request_while_user_is_not_the_reciver_of_request()
        {
            AuthenticationService.Login();

            string userId = AuthenticationService.GetCurrentUser()!.Id;

            var sender = await CreateFakeUser(userId);
            var reciver = await CreateFakeUser();

            var fakeFriendRequest = await CreateFakeFriendRequest(sender, reciver);

            var command = new AcceptFriendRequestCommand
            {
                FriendRequestId = fakeFriendRequest.Id
            };

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

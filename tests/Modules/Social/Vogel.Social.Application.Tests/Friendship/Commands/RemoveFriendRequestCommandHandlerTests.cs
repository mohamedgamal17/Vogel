using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Social.Application.Friendship.Commands.RemoveFriend;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.Domain.Users;
using Vogel.Social.MongoEntities.Friendship;
using Vogel.Social.Shared.Common;
namespace Vogel.Social.Application.Tests.Friendship.Commands
{
    public class RemoveFriendRequestCommandHandlerTests : SocialTestFixture
    {
        public ISocialRepository<User> UserRepository { get; }
        public ISocialRepository<Friend> FriendRepository { get; }
        public FriendMongoRepository FriendMongoRepository { get; }

        public RemoveFriendRequestCommandHandlerTests()
        {
            FriendRepository = ServiceProvider.GetRequiredService<ISocialRepository<Friend>>();
            FriendMongoRepository = ServiceProvider.GetRequiredService<FriendMongoRepository>();
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
        }

        [Test]
        public async Task Should_remove_friend()
        {
            AuthenticationService.Login();

            string userId = AuthenticationService.GetCurrentUser()!.Id;

            var soruce = await CreateFakeUser(userId);

            var reciver = await CreateFakeUser();

            var fakeFriend = await CreateFakeFriend(soruce, reciver);

            var command = new RemoveFriendCommand { Id = fakeFriend.Id };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var friend = await FriendRepository.FindByIdAsync(fakeFriend.Id);

            friend.Should().BeNull();

            var mongoEntity = await FriendMongoRepository.FindByIdAsync(fakeFriend.Id);

            mongoEntity.Should().BeNull();
        }

        [Test]
        public async Task Should_failure_when_removing_friend_while_user_is_not_authorized()
        {

            var source = await CreateFakeUser();
            var target = await CreateFakeUser();

            var fakeFriend = await CreateFakeFriend(source, target);

            var command = new RemoveFriendCommand { Id = fakeFriend.Id };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        [Test]
        public async Task Should_failure_when_removing_friend_while_user_is_not_the_soruce_or_target()
        {
            AuthenticationService.Login();

            var source = await CreateFakeUser();

            var target = await CreateFakeUser();

            var fakeFriend = await CreateFakeFriend(source, target);

            var command = new RemoveFriendCommand { Id = fakeFriend.Id };

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

        private async Task<Friend> CreateFakeFriend(User source, User target)
        {
            var friend = new Friend
            {
                SourceId = source.Id,
                TargetId = target.Id,

            };

            return await FriendRepository.InsertAsync(friend);
        }

    }
}

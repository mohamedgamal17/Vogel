using Microsoft.Extensions.DependencyInjection;
using Vogel.MediaEngine.Shared.Dtos;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Social.Domain.Users;
using Vogel.Social.Domain;
using Vogel.Social.MongoEntities.Users;
using Bogus;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Social.Application.Users.Commands.UpdateUser;
using Vogel.Application.Tests.Extensions;
using FluentAssertions;
using Vogel.Social.Application.Tests.Extensions;
using Vogel.Social.Application.Tests.Fakers;
using Vogel.MediaEngine.Shared.Enums;
namespace Vogel.Social.Application.Tests.Users.Commands
{
    public class UpdateUserCommandHandlerTests : SocialTestFixture
    {
        protected ISocialRepository<User> UserRepository { get; }
        protected IMongoRepository<UserMongoEntity> UserMongoRepository { get; }
        protected FakeMediaService FakeMediaService { get; }

        public UpdateUserCommandHandlerTests()
        {
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
            UserMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<UserMongoEntity>>();
            FakeMediaService = ServiceProvider.GetRequiredService<FakeMediaService>();
        }


        [Test]
        public async Task Should_update_user()
        {
            AuthenticationService.Login();

            var fakeUser = await CreateUserAsync(AuthenticationService.GetCurrentUser().Id);

            var fakePhoto = CreateMedia(AuthenticationService.GetCurrentUser().Id);

            var command = new UpdateUserCommand
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                AvatarId = fakePhoto.Id,
                BirthDate = DateTime.Now.AddYears(-18),
                Gender = Shared.Common.Gender.Female
            };


            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var user = await UserRepository.SingleOrDefaultAsync(x => x.Id == result.Value!.Id);

            var mongoEntity = await UserMongoRepository.FindByIdAsync(user!.Id);

            user.Should().NotBeNull();

            mongoEntity.Should().NotBeNull();

            user.AssertUser(command);

            mongoEntity!.AssertUserMongoEntity(user!);

            result.Value!.AssertUserDto(user!, fakePhoto);
        }


        [Test]
        public async Task Should_failure_while_updating_user_when_user_is_not_authorized()
        {

            var fakePhoto = CreateMedia(Guid.NewGuid().ToString());

            var command = new UpdateUserCommand
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                AvatarId = fakePhoto.Id,
                BirthDate = DateTime.Now.AddYears(-18),
                Gender = Shared.Common.Gender.Female
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        [Test]
        public async Task Should_failure_while_updating_user_when_user_is_dose_not_own_media()
        {
            AuthenticationService.Login();

            await CreateUserAsync(AuthenticationService.GetCurrentUser().Id);

            var fakePhoto = CreateMedia(Guid.NewGuid().ToString());

            var command = new UpdateUserCommand
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                AvatarId = fakePhoto.Id,
                BirthDate = DateTime.Now.AddYears(-18),
                Gender = Shared.Common.Gender.Female
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }

        [Test]
        public async Task Should_failure_while_updating_user_when_avatar_is_not_image()
        {
            AuthenticationService.Login();

            await CreateUserAsync(AuthenticationService.GetCurrentUser().Id);

            var fakeVideo = FakeMediaService.AddMedia(AuthenticationService.GetCurrentUser().Id, MediaType.Video);

            var command = new UpdateUserCommand
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                AvatarId = fakeVideo.Id,
                BirthDate = DateTime.Now.AddYears(-18),
                Gender = Shared.Common.Gender.Female
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(BusinessLogicException));
        }

        [Test]
        public async Task Should_failure_while_updaing_user_when_user_is_not_exist()
        {
            AuthenticationService.Login();

            var fakePhoto = CreateMedia(AuthenticationService.GetCurrentUser().Id);

            var command = new UpdateUserCommand
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                AvatarId = fakePhoto.Id,
                BirthDate = DateTime.Now.AddYears(-18),
                Gender = Shared.Common.Gender.Female
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        [Test]
        public async Task Should_failure_while_updating_user_when_image_is_not_exist()
        {
            AuthenticationService.Login();

            await CreateUserAsync(AuthenticationService.GetCurrentUser().Id);

            var command = new UpdateUserCommand
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                AvatarId = Guid.NewGuid().ToString(),
                BirthDate = DateTime.Now.AddYears(-18),
                Gender = Shared.Common.Gender.Female
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        private async Task<User> CreateUserAsync(string userId)
        {
            var picture = CreateMedia(userId);

            Faker faker = new Faker();

            var user = new User
            {
                Id = userId,
                FirstName = faker.Person.FirstName,
                LastName = faker.Person.LastName,
                BirthDate = DateTime.Now,
                Gender = Shared.Common.Gender.Male,
                AvatarId = picture.Id,
            };

            return await UserRepository.InsertAsync(user);
        }

        private PublicMediaFileDto CreateMedia(string userId)
        {
            return FakeMediaService.AddMedia(userId);
        }
    }
}

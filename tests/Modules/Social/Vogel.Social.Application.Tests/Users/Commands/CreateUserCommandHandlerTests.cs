using Bogus;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Social.Application.Tests.Extensions;
using Vogel.Social.Application.Tests.Fakers;
using Vogel.Social.Application.Users.Commands.CreateUser;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Users;
using Vogel.MediaEngine.Shared.Dtos;
using Vogel.MediaEngine.Shared.Enums;
using Vogel.Social.MongoEntities.Users;
namespace Vogel.Social.Application.Tests.Users.Commands
{
    public class CreateUserCommandHandlerTests : SocialTestFixture
    {
        protected ISocialRepository<User> UserRepository { get; }
        protected IMongoRepository<UserMongoEntity> UserMongoRepository { get; }
        protected FakeMediaService FakeMediaService { get; }

        public CreateUserCommandHandlerTests()
        {
            UserRepository = ServiceProvider.GetRequiredService<ISocialRepository<User>>();
            UserMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<UserMongoEntity>>();
            FakeMediaService = ServiceProvider.GetRequiredService<FakeMediaService>();
        }

        [Test]
        public async Task Should_create_user_profile()
        {
            AuthenticationService.Login();

            var fakePicture = CreateMedia(AuthenticationService.GetCurrentUser().Id);

            var command = new CreateUserCommand
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                AvatarId = fakePicture.Id,
                BirthDate = DateTime.Now.AddYears(-18),
                Gender = Shared.Common.Gender.Male
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var user = await UserRepository.SingleOrDefaultAsync(x => x.Id == result.Value!.Id);

            var mongoEntity = await UserMongoRepository.FindByIdAsync(user!.Id);

            user.Should().NotBeNull();
            mongoEntity.Should().NotBeNull();

            user.AssertUser(command);
            mongoEntity!.AssertUserMongoEntity(user);
            result.Value!.AssertUserDto(user, fakePicture);
        }


        [Test]
        public async Task Should_failure_while_creating_user_when_user_dose_not_own_image()
        {
            AuthenticationService.Login();

            var fakePicture = CreateMedia(Guid.NewGuid().ToString());

            var command = new CreateUserCommand
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                AvatarId = fakePicture.Id,
                BirthDate = DateTime.Now.AddYears(-18),
                Gender = Shared.Common.Gender.Male
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }

        [Test]
        public async Task Should_failure_while_creating_user_when_avatar_is_not_image()
        {
            AuthenticationService.Login();

            var fakeVideo = FakeMediaService.AddMedia(AuthenticationService.GetCurrentUser().Id, MediaType.Video);

            var command = new CreateUserCommand
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                AvatarId = fakeVideo.Id,
                BirthDate = DateTime.Now.AddYears(-18),
                Gender = Shared.Common.Gender.Male
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(BusinessLogicException));
        }

        [Test]
        public async Task Should_failure_while_creating_user_when_user_profile_is_already_created()
        {
            AuthenticationService.Login();

            await CreateUserAsync(AuthenticationService.GetCurrentUser().Id);

            var command = new CreateUserCommand
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                BirthDate = DateTime.Now.AddYears(-18),
                Gender = Shared.Common.Gender.Male
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(BusinessLogicException));
        }

        [Test]
        public async Task Should_failure_while_creating_user_when_user_is_not_authorized()
        {
            var command = new CreateUserCommand
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                BirthDate = DateTime.Now.AddYears(-18),
                Gender = Shared.Common.Gender.Male
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        public async Task Should_failure_while_creating_user_when_image_is_not_found()
        {
            AuthenticationService.Login();

            var command = new CreateUserCommand
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                BirthDate = DateTime.Now.AddYears(-18),
                Gender = Shared.Common.Gender.Male,
                AvatarId = Guid.NewGuid().ToString()
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

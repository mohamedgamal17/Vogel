using Bogus;
using FluentAssertions;
using FluentAssertions.Extensions;
using System.Security.Claims;
using Vogel.Application.Common.Exceptions;
using Vogel.Application.IntegrationTest.Extensions;
using Vogel.Application.Users.Commands;
using Vogel.Domain.Medias;
using Vogel.Domain.Users;
using static Vogel.Application.IntegrationTest.Testing;
namespace Vogel.Application.IntegrationTest.Users
{
    public class UserCommandHandlerTests : BaseTestFixture
    {
        [Test]
        public async Task Should_create_user()
        {
            await RunAsUserAsync();

            var media = await CreateMediaAsync();

            var command = await PrepareUserCreateCommand(media);

            var result = await SendAsync(command);

            result.IsSuccess.Should().BeTrue();

            var user = await FindByIdAsync<User>(result.Value!.Id);

            user.Should().NotBeNull();

            user?.AssertUser(command);
        }

        [Test]
        public async Task Should_failure_while_creating_user_when_user_dose_not_own_media()
        {
            await RunAsUserAsync();

            var media = await CreateMediaAsync();

            await RunAsUserAsync();

            var command = await PrepareUserCreateCommand(media);

            var result = await SendAsync(command);

            result.IsFailure.Should().BeTrue();

            result.Exception.Should().BeOfType<ForbiddenAccessException>();
        }

        [Test]
        public async Task Should_failure_while_creating_user_when_user_profile_is_already_created()
        {
            await RunAsUserAsync();

            await CreateUserAsync();

            var media = await CreateMediaAsync();

            var command = await PrepareUserCreateCommand(media);

            var result = await SendAsync(command);

            result.IsFailure.Should().BeTrue();

            result.Exception.Should().BeOfType<BusinessLogicException>();
        }

        [Test]
        public async Task Should_failure_while_creating_user_when_user_is_not_authorized()
        {
            RemoveCurrentUser();

            var media = await CreateMediaAsync();

            var command = await PrepareUserCreateCommand(media);

            var result = await SendAsync(command);

            result.IsFailure.Should().BeTrue();

            result.Exception.Should().BeOfType<UnauthorizedAccessException>();
        }

        [Test]
        public async Task Should_update_user()
        {
            await RunAsUserAsync();

            var fakeUser = await CreateUserAsync();

            var media = await CreateMediaAsync();

            var command = await PrepareUserUpdateCommand(media);

            var result = await SendAsync(command);

            result.IsSuccess.Should().BeTrue();

            var user = await FindByIdAsync<User>(result.Value!.Id);

            user.Should().NotBeNull();

            user?.AssertUser(command);
        }


        [Test]
        public async Task Should_failure_while_updating_user_when_user_is_not_authorized()
        {
            RemoveCurrentUser();

            var media = await CreateMediaAsync();

            var command = await PrepareUserUpdateCommand(media);

            var result = await SendAsync(command);

            result.IsFailure.Should().BeTrue();

            result.Exception.Should().BeOfType<UnauthorizedAccessException>();
        }

        [Test]
        public async Task Should_failure_while_updating_user_when_user_is_dose_not_own_media()
        {
            await RunAsUserAsync();

            var media = await CreateMediaAsync();

            await RunAsUserAsync();

            var fakeUser = await CreateUserAsync();

            var command = await PrepareUserUpdateCommand(media);

            var result = await SendAsync(command);

            result.IsFailure.Should().BeTrue();

            result.Exception.Should().BeOfType<ForbiddenAccessException>();
        }

        [Test]
        public async Task Should_failure_while_updaing_user_when_user_is_not_exist()
        {
            await RunAsUserAsync();

            var media = await CreateMediaAsync();

            var command = await PrepareUserUpdateCommand(media);

            var result = await SendAsync(command);

            result.IsFailure.Should().BeTrue();

            result.Exception.Should().BeOfType<EntityNotFoundException>();
        }
        private async Task<CreateUserCommand> PrepareUserCreateCommand(Media media)
        {
            Faker faker = new Faker();

            var command = new CreateUserCommand
            {
                FirstName = faker.Person.FirstName,
                LastName = faker.Person.LastName,
                BirthDate = faker.Person.DateOfBirth,
                Gender = Gender.Male,
                AvatarId = media.Id
            };
            return command;
        }

        private async Task<UpdateUserCommand> PrepareUserUpdateCommand(Media media)
        {

            Faker faker = new Faker();

            var command = new UpdateUserCommand
            {
                FirstName = faker.Person.FirstName,
                LastName = faker.Person.LastName,
                BirthDate = faker.Person.DateOfBirth,
                Gender = Gender.Male,
                AvatarId = media.Id
            };
            return command;
        }

        private async Task<User> CreateUserAsync()
        {
            var media = await CreateMediaAsync();

            Faker faker = new Faker();

            var user = new User
            {
                Id = CurrentUser?.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? Guid.NewGuid().ToString(),
                FirstName = faker.Person.FirstName,
                LastName = faker.Person.LastName,
                BirthDate = DateTime.Now,
                Gender = Gender.Male,
                AvatarId = media.Id,
            };

            return await InsertAsync(user);
        }

        private async Task<Media> CreateMediaAsync()
        {
            var media = new Media()
            {
                MediaType = MediaType.Image,
                Size = 56666,
                File = Guid.NewGuid().ToString(),
                MimeType = "image/png",
                UserId = CurrentUser?.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? Guid.NewGuid().ToString()
            };

            return await InsertAsync(media);
        }
    }
}

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Repositories;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Social.Application.Pictures.Commands.RemovePicture;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Pictures;
using Vogel.Social.MongoEntities.Pictures;
namespace Vogel.Social.Application.Tests.Pictures.Commands
{
    public class RemovePictureCommandHandlerTests : SocialTestFixture
    {
        public ISocialRepository<Picture> PictureRepository { get; private set; }
        public IMongoRepository<PictureMongoEntity> PictureMongoRepository { get; private set; }

        public RemovePictureCommandHandlerTests()
        {
            PictureRepository = ServiceProvider.GetRequiredService<ISocialRepository<Picture>>();
            PictureMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<PictureMongoEntity>>();
        }

        [Test]
        public async Task Should_remove_picture()
        {
            AuthenticationService.Login();

            var fakePicture = await CreatePicture(AuthenticationService.GetCurrentUser().Id);

            var command = new RemovePictureCommand
            {
                Id = fakePicture.Id
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var picture = await PictureRepository.SingleOrDefaultAsync(x => x.Id == fakePicture.Id);
            var mongoEntity = await PictureMongoRepository.FindByIdAsync(fakePicture.Id);
            picture.Should().BeNull();
            mongoEntity.Should().BeNull();
        }

        [Test]
        public async Task Should_failure_while_removing_image_when_user_is_not_authorized()
        {
            var command = new RemovePictureCommand
            {
                Id = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        [Test]
        public async Task Should_failure_while_removing_image_when_user_is_not_the_owner()
        {
            AuthenticationService.Login();

            var fakePicture = await CreatePicture(AuthenticationService.GetCurrentUser().Id);

            AuthenticationService.Login();

            var command = new RemovePictureCommand
            {
                Id = fakePicture.Id
            };

            var result = await Mediator.Send(command);
            result.ShoulBeFailure(typeof(ForbiddenAccessException));
        }

        [Test]
        public async Task Should_failure_while_removing_image_when_image_is_not_exist()
        {
            AuthenticationService.Login();

            var command = new RemovePictureCommand
            {
                Id = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(EntityNotFoundException));
        }

        private async Task<Picture> CreatePicture(string userId)
        {
            var picture = new Picture
            {
                File = Guid.NewGuid().ToString(),
                UserId = userId
            };

            return await PictureRepository.InsertAsync(picture);
        }

    }
}

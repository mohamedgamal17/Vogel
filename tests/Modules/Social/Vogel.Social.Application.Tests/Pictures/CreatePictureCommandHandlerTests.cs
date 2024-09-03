using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.Domain.Repositories;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Social.Application.Pictures.Commands.CreatePicture;
using Vogel.Social.Application.Tests.Extensions;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Pictures;
using Vogel.Social.MongoEntities.Pictures;
namespace Vogel.Social.Application.Tests.Pictures
{
    public class CreatePictureCommandHandlerTests : SocialTestFixture
    {
        public ISocialRepository<Picture> PictureRepository { get; private set; }
        public IMongoRepository<PictureMongoEntity> PictureMongoRepository { get; private set; }

        public CreatePictureCommandHandlerTests()
        {
            PictureRepository = ServiceProvider.GetRequiredService<ISocialRepository<Picture>>();
            PictureMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<PictureMongoEntity>>();
        }

        [Test]
        public async Task Should_upload_picture()
        {
            UserService.Login();

            var command = new CreatePictureCommand
            {
                Name = Guid.NewGuid().ToString(),
                Content = await Resource.LoadImageAsStream(),
                MimeType = "img/png"
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var picture = await PictureRepository.SingleOrDefaultAsync(x => x.Id == result.Value!.Id);
            var mongoEntity = await PictureMongoRepository.FindByIdAsync(result.Value!.Id);

            picture.Should().NotBeNull();
            mongoEntity.Should().NotBeNull();
            picture!.AssertPicture(command, UserService.GetCurrentUser().Id);
            mongoEntity!.AssertPictureMongoEntity(picture!);
            result.Value.AssertPictureDto(picture!);
        }

        [Test]
        public async Task Should_failure_while_uploading_image_when_user_is_not_authorized()
        {
            var command = new CreatePictureCommand
            {
                Name = Guid.NewGuid().ToString(),
                Content = await Resource.LoadImageAsStream(),
                MimeType = "img/png"
            };
            var result = await Mediator.Send(command);
            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }
    }
}

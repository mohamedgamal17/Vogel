using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.Application.Medias.Commands.CreateMedia;
using Vogel.Content.Application.Tests.Extensions;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Medias;
using Vogel.Content.MongoEntities.Medias;
namespace Vogel.Content.Application.Tests.Medias
{
    public class CreateMediaCommandHandlerTests : ContentTestFixture
    {
        public IContentRepository<Media> MediaRepository { get; }
        public IMongoRepository<MediaMongoEntity> MediaMongoRepository { get; }
        public CreateMediaCommandHandlerTests()
        {
            MediaRepository = ServiceProvider.GetRequiredService<IContentRepository<Media>>();
            MediaMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<MediaMongoEntity>>();
        }

        [Test]
        public async Task Should_upload_user_media()
        {
            AuthenticationService.Login();

            var command = new CreateMediaCommand
            {
                Content = await Resource.LoadImageAsStream(),
                MediaType = Domain.Medias.MediaType.Image,
                MimeType = "image/png",
                Name = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var media = await MediaRepository.FindByIdAsync(result.Value!.Id);

            var mediaMongoEntity = await MediaMongoRepository.FindByIdAsync(media!.Id);

            mediaMongoEntity.Should().NotBeNull();

            media!.AssertMedia(command, AuthenticationService.GetCurrentUser()!.Id);

            media.AssertMediaMongoEntity(mediaMongoEntity!);

            result.Value.AssertMediaDto(media);
        }


        [Test]
        public async Task Should_failure_while_creating_media_when_user_is_not_authorized()
        {
            var command = new CreateMediaCommand
            {
                Content = await Resource.LoadImageAsStream(),
                MediaType = Domain.Medias.MediaType.Image,
                MimeType = "image/png",
                Name = Guid.NewGuid().ToString()
            };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

 
    }
}

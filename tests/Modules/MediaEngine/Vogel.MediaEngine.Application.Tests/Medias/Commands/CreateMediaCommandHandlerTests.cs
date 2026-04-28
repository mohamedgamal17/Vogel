using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.MediaEngine.Application.Medias.Commands.CreateMedia;
using Vogel.MediaEngine.Domain;
using Vogel.MediaEngine.Domain.Medias;
using Vogel.MediaEngine.MongoEntities.Medias;
using DomainMediaType = Vogel.MediaEngine.Domain.Medias.MediaType;

namespace Vogel.MediaEngine.Application.Tests.Medias.Commands
{
    public class CreateMediaCommandHandlerTests : MediaEngineTestFixture
    {
        private readonly IMediaEngineRepository<Media> _mediaRepository;
        private readonly IMongoRepository<MediaMongoEntity> _mediaMongoRepository;

        public CreateMediaCommandHandlerTests()
        {
            _mediaRepository = ServiceProvider.GetRequiredService<IMediaEngineRepository<Media>>();
            _mediaMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<MediaMongoEntity>>();
        }

        [Test]
        public async Task Should_upload_media_for_current_user()
        {
            AuthenticationService.Login("user-1", "user-1", new List<string>());

            var command = new CreateMediaCommand
            {
                Name = Guid.NewGuid().ToString(),
                MimeType = "image/png",
                MediaType = DomainMediaType.Image,
                Content = new MemoryStream(new byte[] { 1, 2, 3, 4 }),
            };

            var result = await Mediator.Send(command);
            result.ShouldBeSuccess();

            var media = await _mediaRepository.FindByIdAsync(result.Value!.Id);
            media.Should().NotBeNull();

            var mongo = await _mediaMongoRepository.FindByIdAsync(result.Value!.Id);
            mongo.Should().NotBeNull();
        }
    }
}

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.MediaEngine.Application.Medias.Commands.RemoveMedia;
using Vogel.MediaEngine.Domain;
using Vogel.MediaEngine.Domain.Medias;
using Vogel.MediaEngine.MongoEntities.Medias;
using DomainMediaType = Vogel.MediaEngine.Domain.Medias.MediaType;

namespace Vogel.MediaEngine.Application.Tests.Medias.Commands
{
    public class RemoveMediaCommandHandlerTests : MediaEngineTestFixture
    {
        private readonly IMediaEngineRepository<Media> _mediaRepository;
        private readonly IMongoRepository<MediaMongoEntity> _mediaMongoRepository;

        public RemoveMediaCommandHandlerTests()
        {
            _mediaRepository = ServiceProvider.GetRequiredService<IMediaEngineRepository<Media>>();
            _mediaMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<MediaMongoEntity>>();
        }

        [Test]
        public async Task Should_remove_media()
        {
            var media = new Media
            {
                File = Guid.NewGuid().ToString(),
                MimeType = "image/png",
                MediaType = DomainMediaType.Image,
                Size = 100,
                UserId = "user-2",
            };

            await _mediaRepository.InsertAsync(media);
            AuthenticationService.Login("user-2", "user-2", new List<string>());

            var result = await Mediator.Send(new RemoveMediaCommand { Id = media.Id });
            result.ShouldBeSuccess();

            (await _mediaRepository.FindByIdAsync(media.Id)).Should().BeNull();
            (await _mediaMongoRepository.FindByIdAsync(media.Id)).Should().BeNull();
        }
    }
}

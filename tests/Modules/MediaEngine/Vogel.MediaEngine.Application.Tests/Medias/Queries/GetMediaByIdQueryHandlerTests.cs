using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Application.Tests.Extensions;
using Vogel.MediaEngine.Application.Medias.Queries.GetMediaById;
using Vogel.MediaEngine.Domain;
using Vogel.MediaEngine.Domain.Medias;

namespace Vogel.MediaEngine.Application.Tests.Medias.Queries
{
    public class GetMediaByIdQueryHandlerTests : MediaEngineTestFixture
    {
        private readonly IMediaEngineRepository<Media> _mediaRepository;

        public GetMediaByIdQueryHandlerTests()
        {
            _mediaRepository = ServiceProvider.GetRequiredService<IMediaEngineRepository<Media>>();
        }

        [Test]
        public async Task Should_get_owned_media_by_id()
        {
            var media = new Media
            {
                File = Guid.NewGuid().ToString(),
                MimeType = "image/png",
                MediaType = MediaType.Image,
                Size = 10,
                UserId = "user-3",
            };
            await _mediaRepository.InsertAsync(media);
            AuthenticationService.Login("user-3", "user-3", new List<string>());

            var result = await Mediator.Send(new GetMediaByIdQuery { MediaId = media.Id });
            result.ShouldBeSuccess();
            result.Value!.Id.Should().Be(media.Id);
        }
    }
}

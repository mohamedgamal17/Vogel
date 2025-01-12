using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.Domain.Medias;
using Vogel.Content.Domain;
using Vogel.Content.MongoEntities.Medias;
using Microsoft.Extensions.DependencyInjection;
using Vogel.Content.Application.Medias.Commands.RemoveMedia;
using Vogel.Application.Tests.Extensions;
using FluentAssertions;
using Vogel.BuildingBlocks.Domain.Exceptions;

namespace Vogel.Content.Application.Tests.Medias
{
    public class RemoveMediaCommandHandlerTests : ContentTestFixture
    {
        public IContentRepository<Media> MediaRepository { get; }
        public IMongoRepository<MediaMongoEntity> MediaMongoRepository { get; }

        public RemoveMediaCommandHandlerTests()
        {
            MediaRepository = ServiceProvider.GetRequiredService<IContentRepository<Media>>();
            MediaMongoRepository = ServiceProvider.GetRequiredService<IMongoRepository<MediaMongoEntity>>();
        }

        [Test]
        public async Task Should_remove_media()
        {
            AuthenticationService.Login();

            string userId = AuthenticationService.GetCurrentUser()!.Id;

            var fakeMedia = await CreateMediaAsync(userId);

            var command = new RemoveMediaCommand
            {
                Id = fakeMedia.Id
            };

            var result = await Mediator.Send(command);

            result.ShouldBeSuccess();

            var media = await MediaRepository.FindByIdAsync(fakeMedia.Id);

            media.Should().BeNull();

            var mongoEntity = await MediaMongoRepository.FindByIdAsync(fakeMedia.Id);

            mongoEntity.Should().BeNull();
        }

        [Test]
        public async Task Should_failure_while_removing_media_when_user_is_not_authorized()
        {
            var media = await CreateMediaAsync(Guid.NewGuid().ToString());

            var command = new RemoveMediaCommand() { Id = media.Id };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(UnauthorizedAccessException));
        }

        [Test]
        public async Task Should_failure_while_removing_media_when_user_dose_not_own_this_resource()
        {
            var media = await CreateMediaAsync(Guid.NewGuid().ToString());

            AuthenticationService.Login();

            var command = new RemoveMediaCommand() { Id = media.Id };

            var result = await Mediator.Send(command);

            result.ShoulBeFailure(typeof(ForbiddenAccessException));

        }

        private async Task<Media> CreateMediaAsync(string userId)
        {
            var media = new Media()
            {
                MediaType = Domain.Medias.MediaType.Image,
                Size = 56666,
                File = Guid.NewGuid().ToString(),
                MimeType = "image/png",
                UserId = userId
            };

            return await MediaRepository.InsertAsync(media);
        }
    }
}

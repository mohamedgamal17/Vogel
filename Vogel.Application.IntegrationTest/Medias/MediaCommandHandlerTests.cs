using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Security.Claims;
using Vogel.Application.IntegrationTest.Extensions;
using Vogel.Application.IntegrationTest.Utilites;
using Vogel.Application.Medias.Commands;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Domain.Medias;
using Vogel.MongoDb.Entities.Medias;
using static Vogel.Application.IntegrationTest.Testing;
namespace Vogel.Application.IntegrationTest.Medias
{
    public class MediaCommandHandlerTests : BaseTestFixture
    {
        public MediaMongoRepository MediaMongoRepository { get; set; }
        public MediaCommandHandlerTests()
        {
            MediaMongoRepository = Testing.ServiceProvider.GetRequiredService<MediaMongoRepository>();
        }

        [Test]
        public async Task Should_create_media()
        {
            await RunAsUserWithProfile();

            var command = await PrepareCreateMediaCommand();

            var result = await SendAsync(command);

            result.IsSuccess.Should().BeTrue();

            var media = await FindByIdAsync<Media>(result.Value!.Id);

            var mediaMongoEntity = await MediaMongoRepository.FindByIdAsync(media!.Id);

            mediaMongoEntity.Should().NotBeNull();

            media!.AssertMedia(command);

            media.AssertMediaMongoEntity(mediaMongoEntity!);
     
        }

        [Test]
        public async Task Should_failure_while_creating_media_when_user_is_not_authorized()
        {
            RemoveCurrentUser();

            var command = await PrepareCreateMediaCommand();

            var result = await SendAsync(command);

            result.IsSuccess.Should().BeFalse();

            result.Exception.Should().BeOfType<UnauthorizedAccessException>();
        }


        [Test]
        public async Task Should_remove_media()
        {
            var fakeMedia = await CreateMediaAsync();

            var command = new RemoveMediaCommand() { Id = fakeMedia.Id };

            var result = await SendAsync(command);

            result.IsSuccess.Should().BeTrue();

            var media = await FindByIdAsync<Media>(fakeMedia.Id);

            media.Should().BeNull();

            var mediaMongoEntity = await MediaMongoRepository.FindByIdAsync(command.Id);

            mediaMongoEntity.Should().BeNull();

        }

        [Test]
        public async Task Should_failure_while_removing_media_when_user_is_not_authorized()
        {
            var media = await CreateMediaAsync();

            RemoveCurrentUser();

            var command = new RemoveMediaCommand() { Id = media.Id };

            var result = await SendAsync(command);

            result.IsSuccess.Should().BeFalse();

            result.Exception.Should().BeOfType<UnauthorizedAccessException>();

        }

        [Test]
        public async Task Should_failure_while_removing_media_when_user_dose_not_own_this_resource()
        {
            var media = await CreateMediaAsync();

            await RunAsUserAsync();

            var command = new RemoveMediaCommand() { Id = media.Id };

            var result = await SendAsync(command);

            result.IsSuccess.Should().BeFalse();

            result.Exception.Should().BeOfType<ForbiddenAccessException>();

        }

        private async Task<CreateMediaCommand> PrepareCreateMediaCommand()
        {
            var stream = await Resource.LoadImageAsStream();

            var command = new CreateMediaCommand
            {
                Content = stream,
                MediaType = Domain.Medias.MediaType.Image,
                MimeType = "image/png",
                Name = Guid.NewGuid().ToString()
            };

            return command;
        }

        private async Task<Media> CreateMediaAsync()
        {
            await RunAsUserWithProfile();

            var media = new Media()
            {
                MediaType = Domain.Medias.MediaType.Image,
                Size = 56666,
                File = Guid.NewGuid().ToString(),
                MimeType = "image/png",
                UserId = CurrentUser!.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value
            };

            return await InsertAsync(media);
        }
    }
}

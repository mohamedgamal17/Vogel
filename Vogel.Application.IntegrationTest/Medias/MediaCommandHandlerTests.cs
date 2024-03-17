using FluentAssertions;
using MongoDB.Driver;
using System.Security.Claims;
using Vogel.Application.Common.Exceptions;
using Vogel.Application.IntegrationTest.Extensions;
using Vogel.Application.IntegrationTest.Utilites;
using Vogel.Application.Medias.Commands;
using Vogel.Domain;
using static Vogel.Application.IntegrationTest.Testing;
namespace Vogel.Application.IntegrationTest.Medias
{
    public class MediaCommandHandlerTests : BaseTestFixture
    {
        [Test]
        public async Task Should_create_media()
        {
            await RunAsUserAsync();

            var command = await PrepareCreateMediaCommand();

            var result = await SendAsync(command);

            result.IsSuccess.Should().BeTrue();

            var media = await FindByIdAsync<Media>(result.Value!.Id);

            media!.AssertMedia(command);
     
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
                MediaType = Domain.MediaType.Image,
                MimeType = "image/png",
                Name = Guid.NewGuid().ToString()
            };

            return command;
        }

        private async Task<Media> CreateMediaAsync()
        {
            await RunAsUserAsync();

            var media = new Media()
            {
                MediaType = MediaType.Image,
                Size = 56666,
                File = Guid.NewGuid().ToString(),
                MimeType = "image/png",
                UserId = CurrentUser!.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value
            };

            return await InsertAsync(media);
        }
    }
}

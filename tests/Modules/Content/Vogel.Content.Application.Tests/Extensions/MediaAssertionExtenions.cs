using FluentAssertions;
using Vogel.Application.Tests.Extensions;
using Vogel.Content.Application.Medias.Commands.CreateMedia;
using Vogel.Content.Application.Medias.Dtos;
using Vogel.Content.Domain.Medias;
using Vogel.Content.MongoEntities.Medias;
namespace Vogel.Content.Application.Tests.Extensions
{
    public static class MediaAssertionExtenions
    {
        public static void AssertMedia(this Media media, CreateMediaCommand command , string userId)
        {
            media.File.Should().Be(command.Name);
            media.MimeType.Should().Be(command.MimeType);
            media.UserId.Should().Be(userId);
        }

        public static void AssertMediaMongoEntity(this Media media, MediaMongoEntity mongoEntity)
        {
            media.Id.Should().Be(mongoEntity.Id);
            media.UserId.Should().Be(mongoEntity.UserId);
            media.MediaType.Should().Be((Domain.Medias.MediaType)mongoEntity.MediaType);
            media.MimeType.Should().Be(mongoEntity.MimeType);
            media.File.Should().Be(mongoEntity.File);
            media.AssertAuditingProperties(mongoEntity);
        }

        public static void AssertMediaDto(this MediaDto dto, Media media)
        {
            dto.Id.Should().Be(media.Id);
            dto.UserId.Should().Be(media.UserId);
            dto.MimeType.Should().Be(media.MimeType);
            dto.MediaType.Should().Be((MongoEntities.Medias.MediaType)media.MediaType);
        }
    }
}

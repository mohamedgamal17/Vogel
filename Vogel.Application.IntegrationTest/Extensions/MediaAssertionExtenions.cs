using FluentAssertions;
using System.Security.Claims;
using Vogel.Application.Medias.Commands;
using Vogel.Domain.Medias;
using Vogel.MongoDb.Entities.Medias;
using static Vogel.Application.IntegrationTest.Testing;
namespace Vogel.Application.IntegrationTest.Extensions
{
    public static class MediaAssertionExtenions
    {
        public static void AssertMedia(this Media media, CreateMediaCommand command)
        {
            string userId = CurrentUser!.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;

            media.File.Should().Be(command.Name);
            media.MimeType.Should().Be(command.MimeType);
            media.UserId.Should().Be(userId);
        }

        public static void AssertMediaMongoEntity(this Media media , MediaMongoEntity mongoEntity)
        {
            media.Id.Should().Be(mongoEntity.Id);
            media.UserId.Should().Be(mongoEntity.UserId);
            media.MediaType.Should().Be((Domain.Medias.MediaType)mongoEntity.MediaType);
            media.MimeType.Should().Be(mongoEntity.MimeType);
            media.File.Should().Be(mongoEntity.File);
            media.AssertAuditingProperties(mongoEntity);
        }
    }
}

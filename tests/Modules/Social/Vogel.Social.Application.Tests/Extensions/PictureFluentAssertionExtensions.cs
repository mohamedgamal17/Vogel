using FluentAssertions;
using Vogel.Social.Application.Pictures.Commands.CreatePicture;
using Vogel.Social.Domain.Pictures;
using Vogel.Social.MongoEntities.Pictures;
using Vogel.Application.Tests.Extensions;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Social.Application.Tests.Extensions
{
    public static class PictureFluentAssertionExtensions
    {
        public static void AssertPicture(this Picture picture, CreatePictureCommand command , string userId)
        {
            picture.File.Should().Be(command.Name);
            picture.UserId.Should().Be(userId);
        }

        public static void AssertPictureMongoEntity(this Picture picture, PictureMongoEntity mongoEntity)
        {
            picture.Id.Should().Be(mongoEntity.Id);
            picture.UserId.Should().Be(mongoEntity.UserId);
            picture.File.Should().Be(mongoEntity.File);
            picture.AssertAuditingProperties(mongoEntity);
        }

        public static void AssertPictureDto(this PictureDto dto, Picture picture)
        {
            dto.Id.Should().Be(picture.Id);
            dto.UserId.Should().Be(picture.UserId);
        }
    }
}

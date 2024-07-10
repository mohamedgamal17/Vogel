using FluentAssertions;
using System.Security.Claims;
using Vogel.Application.Posts.Commands;
using Vogel.Application.Posts.Dtos;
using Vogel.Domain.Medias;
using Vogel.Domain.Posts;
using Vogel.Domain.Users;
using Vogel.MongoDb.Entities.Posts;
using static Vogel.Application.IntegrationTest.Testing;
namespace Vogel.Application.IntegrationTest.Extensions
{
    public static class PostAssertionExtenions
    {
        public static void AssertPost(this Post post, PostCommandBase command)
        {
            string userId = CurrentUser!.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
            post.Caption.Should().Be(command.Caption);
            post.MediaId.Should().Be(command.MediaId);
            post.UserId.Should().Be(userId);
        }

        public static void AssertPostMongoEntity(this Post post, PostMongoEntity mongoEntity)
        {
            post.Id.Should().Be(mongoEntity.Id);
            post.Caption.Should().Be(mongoEntity.Caption);
            post.MediaId.Should().Be(mongoEntity.MediaId);
            post.AssertAuditingProperties(mongoEntity);
        }

        public static void AssertPostDto(this PostAggregateDto dto, Post post, UserAggregate? user = null , Media? media = null)
        {
            dto.Id.Should().Be(post.Id);
            dto.Caption.Should().Be(post.Caption);
            dto.MediaId.Should().Be(post.MediaId);
            dto.UserId.Should().Be(post.UserId);

            if(user != null)
            {
                dto.User.Should().NotBeNull();
                dto.User.AssertPublicUserDto(user);
            }

            if(media != null)
            {
                dto.Media.Should().NotBeNull();
                dto.Media.AssertMediaDto(media);
            }
        }
    }
}

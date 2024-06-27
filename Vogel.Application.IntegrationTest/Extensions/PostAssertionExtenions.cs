using FluentAssertions;
using System.Security.Claims;
using Vogel.Application.Posts.Commands;
using Vogel.Domain.Posts;
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
    }
}

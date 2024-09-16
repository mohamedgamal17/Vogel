using FluentAssertions;
using Vogel.Content.Application.Posts.Commands.CreatePost;
using Vogel.Content.Application.Posts.Commands.UpdatePost;
using Vogel.Content.Application.Posts.Dtos;
using Vogel.Content.Domain.Medias;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.Posts;
namespace Vogel.Content.Application.Tests.Extensions
{
    public static class PostAssertionExtenions
    {
        public static void AssertPostCommand(this Post post, CreatePostCommand command, string userId)
        {
            post.Caption.Should().Be(command.Caption);
            post.MediaId.Should().Be(command.MediaId);
            post.UserId.Should().Be(userId);
        }

        public static void AssertPostCommand(this Post post, UpdatePostCommand command, string userId)
        {
            post.Caption.Should().Be(command.Caption);
            post.MediaId.Should().Be(command.MediaId);
            post.UserId.Should().Be(userId);
        }


        public static void AssertPostMongoEntity(this Post post, PostMongoEntity mongoEntity)
        {
            post.Id.Should().Be(mongoEntity.Id);
            post.Caption.Should().Be(mongoEntity.Caption);
            post.MediaId.Should().Be(mongoEntity.MediaId);
        }

        public static void AssertPostDto(this PostDto dto, Post post, Media? media = null)
        {
            dto.Id.Should().Be(post.Id);
            dto.Caption.Should().Be(post.Caption);
            dto.MediaId.Should().Be(post.MediaId);
            dto.UserId.Should().Be(post.UserId);
            if (media != null)
            {
                dto.Media.Should().NotBeNull();
                dto.Media.AssertMediaDto(media);
            }
        }
    }
}

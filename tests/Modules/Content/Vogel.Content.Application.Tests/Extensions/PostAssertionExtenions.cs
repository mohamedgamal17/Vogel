using FluentAssertions;
using Humanizer;
using Vogel.Content.Application.PostReactions.Dtos;
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
        public static void AssertPost(this Post post, CreatePostCommand command, string userId)
        {
            post.Caption.Should().Be(command.Caption);
            post.MediaId.Should().Be(command.MediaId);
            post.UserId.Should().Be(userId);
        }

        public static void AssertPost(this Post post, UpdatePostCommand command, string userId)
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

        public static void AssertReactionSummaryDto(this PostDto  dto, PostReactionSummaryDto expected)
        {
            dto.ReactionSummary.Should().NotBeNull();
            dto.ReactionSummary.Should().NotBeNull();
            dto.ReactionSummary!.TotalAngry.Should().Be(expected.TotalAngry);
            dto.ReactionSummary!.TotalLaugh.Should().Be(expected.TotalLaugh);
            dto.ReactionSummary!.TotalSad.Should().Be(expected.TotalSad);
            dto.ReactionSummary!.TotalLike.Should().Be(expected.TotalLike);
            dto.ReactionSummary!.TotalLove.Should().Be(expected.TotalLove);
        }
    }
}

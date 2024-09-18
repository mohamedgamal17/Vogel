using FluentAssertions;
using Vogel.Application.Tests.Extensions;
using Vogel.Content.Application.PostReactions.Commands.CreatePostReaction;
using Vogel.Content.Application.PostReactions.Commands.UpdatePostReaction;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.PostReactions;
namespace Vogel.Content.Application.Tests.Extensions
{
    public static class PostReactionAssertionExtensions
    {

        public static void AssertPostReaction(this PostReaction postReaction, CreatePostReactionCommand command , string userId)
        {
            postReaction.PostId.Should().Be(command.PostId);
            postReaction.Type.Should().Be(command.Type);
            postReaction.UserId.Should().Be(userId);
        }

        public static void AssertPostReaction(this PostReaction postReaction, UpdatePostReactionCommand command, string userId)
        {
            postReaction.Type.Should().Be(command.Type);
            postReaction.PostId.Should().Be(command.PostId);
            postReaction.UserId.Should().Be(userId);
        }

        public static void AssertPostReactionMongoEntity(this PostReactionMongoEntity mongoEntity, PostReaction postReaction)
        {
            mongoEntity.Id.Should().Be(postReaction.Id);
            mongoEntity.UserId.Should().Be(postReaction.UserId);
            mongoEntity.PostId.Should().Be(postReaction.PostId);
            mongoEntity.Type.Should().Be((MongoEntities.PostReactions.ReactionType)postReaction.Type);
            mongoEntity.AssertAuditingProperties(postReaction);
        }

        public static void AssertPostReactionDto(this PostReactionDto dto, PostReaction postReaction)
        {
            dto.Id.Should().Be(postReaction.Id);
            dto.PostId.Should().Be(postReaction.PostId);
            dto.UserId.Should().Be(postReaction.UserId);
            dto.Type.Should().Be((MongoEntities.PostReactions.ReactionType)postReaction.Type);
            dto.User.Should().NotBeNull();
        }
    }
}

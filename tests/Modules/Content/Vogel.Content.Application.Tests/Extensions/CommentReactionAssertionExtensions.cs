using FluentAssertions;
using Vogel.Application.Tests.Extensions;
using Vogel.Content.Application.CommentReactions.Commands.CreateCommentReaction;
using Vogel.Content.Application.CommentReactions.Commands.UpdateCommentReaction;
using Vogel.Content.Application.CommentReactions.Dtos;
using Vogel.Content.Domain.Comments;
using Vogel.Content.MongoEntities.CommentReactions;
namespace Vogel.Content.Application.Tests.Extensions
{
    public static class CommentReactionAssertionExtensions
    {

        public static void AssertCommentReaction(this CommentReaction commentReaction, CreateCommentReactionCommand command , string userId)
        {
            commentReaction.CommentId.Should().Be(command.CommentId);
            commentReaction.Type.Should().Be(command.Type);
            commentReaction.UserId.Should().Be(userId);
        }

        public static void AssertCommentReaction(this CommentReaction commentReaction , UpdateCommentReactionCommand command, string userId)
        {
            commentReaction.CommentId.Should().Be(command.CommentId);
            commentReaction.Type.Should().Be(command.Type);
            commentReaction.UserId.Should().Be(userId);
        }

        public static void AssertCommentReactionMongoEntity(this CommentReactionMongoEntity mongoEntity, CommentReaction commentReaction)
        {
            mongoEntity.Id.Should().Be(commentReaction.Id);
            mongoEntity.UserId.Should().Be(commentReaction.UserId);
            mongoEntity.CommentId.Should().Be(commentReaction.CommentId);
            mongoEntity.Type.Should().Be((MongoEntities.PostReactions.ReactionType)commentReaction.Type);
            mongoEntity.AssertAuditingProperties(commentReaction);
        }

        public static void AssertCommentReactionDto(this CommentReactionDto dto, CommentReaction commentReaction)
        {
            dto.Id.Should().Be(commentReaction.Id);
            dto.UserId.Should().Be(commentReaction.UserId);
            dto.CommentId.Should().Be(commentReaction.CommentId);
            dto.Type.Should().Be((MongoEntities.PostReactions.ReactionType)commentReaction.Type);
            dto.User.Should().NotBeNull();
           
        }

    }
}

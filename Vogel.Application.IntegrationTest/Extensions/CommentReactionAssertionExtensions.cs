using FluentAssertions;
using Vogel.Application.CommentReactions.Commands;
using Vogel.Application.CommentReactions.Dtos;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Domain.Comments;
using Vogel.Domain.Users;
using Vogel.MongoDb.Entities.CommentReactions;

namespace Vogel.Application.IntegrationTest.Extensions
{
    public static class CommentReactionAssertionExtensions
    {

        public static void AssertCommentReactionCommand(this CommentReaction commentReaction , CommentReactionCommandBase command)
        {
            commentReaction.CommentId.Should().Be(command.CommentId);
            commentReaction.Type.Should().Be(command.Type);
        }

        public static void AssertCommentReactionMongoEntity(this CommentReactionMongoEntity mongoEntity , CommentReaction commentReaction)
        {
            mongoEntity.Id.Should().Be(commentReaction.Id);
            mongoEntity.UserId.Should().Be(commentReaction.UserId);
            mongoEntity.CommentId.Should().Be(commentReaction.CommentId);
            mongoEntity.Type.Should().Be((MongoDb.Entities.PostReactions.ReactionType)commentReaction.Type);
            mongoEntity.AssertAuditingProperties(commentReaction);
        }

        public static void AssertCommentReactionDto(this CommentReactionDto dto , CommentReaction commentReaction , UserAggregate? user = null)
        {
            dto.Id.Should().Be(commentReaction.Id);
            dto.UserId.Should().Be(commentReaction.UserId);
            dto.CommentId.Should().Be(commentReaction.CommentId);
            dto.Type.Should().Be(((MongoDb.Entities.PostReactions.ReactionType)commentReaction.Type));

            if(user != null)
            {
                dto.User.Should().NotBeNull();

                dto.User.AssertPublicUserDto(user);
            }
        }

    }
}

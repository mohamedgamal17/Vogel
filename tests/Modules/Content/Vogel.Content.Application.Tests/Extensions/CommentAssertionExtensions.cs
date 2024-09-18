using FluentAssertions;
using Vogel.Application.Tests.Extensions;
using Vogel.Content.Application.Comments.Commands.CreateComment;
using Vogel.Content.Application.Comments.Commands.UpdateComment;
using Vogel.Content.Application.Comments.Dtos;
using Vogel.Content.Domain.Comments;
using Vogel.Content.MongoEntities.Comments;
namespace Vogel.Content.Application.Tests.Extensions
{
    public static class CommentAssertionExtensions
    {
        public static void AssertComment(this Comment comment, CreateCommentCommand command, string userId)
        {
            comment.Content.Should().Be(command.Content);
            comment.PostId.Should().Be(command.PostId);
            comment.UserId.Should().Be(userId);
            comment.CommentId.Should().Be(command.CommentId);
        }

        public static void AssertComment(this Comment comment, UpdateCommentCommand command, string userId)
        {
            comment.Id.Should().Be(command.CommentId);
            comment.Content.Should().Be(command.Content);
            comment.PostId.Should().Be(command.PostId);
            comment.UserId.Should().Be(userId);
        }

        public static void AssertCommentMongoEntity(this Comment comment, CommentMongoEntity mongoEntity)
        {
            comment.Id.Should().Be(mongoEntity.Id);
            comment.Content.Should().Be(mongoEntity.Content);
            comment.PostId.Should().Be(mongoEntity.PostId);
            comment.AssertAuditingProperties(mongoEntity);
        }

        public static void AssertCommentDto(this CommentDto dto, Comment comment)
        {
            dto.Id.Should().Be(comment.Id);
            dto.Content.Should().Be(comment.Content);
            dto.PostId.Should().Be(comment.PostId);
            dto.UserId.Should().Be(comment.UserId);
            dto.User.Should().NotBeNull();
        }
    }
}

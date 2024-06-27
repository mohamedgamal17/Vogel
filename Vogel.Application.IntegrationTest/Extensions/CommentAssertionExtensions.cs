using FluentAssertions;
using Vogel.Application.Comments.Commands;
using Vogel.Domain.Posts;
using Vogel.MongoDb.Entities.Comments;

namespace Vogel.Application.IntegrationTest.Extensions
{
    public static class CommentAssertionExtensions
    {
        public static void AssertComment(this Comment comment , CommentCommandBase command)
        {
            comment.Content.Should().Be(command.Content);
        }

        public static void AssertCommentMongoEntity(this Comment comment , CommentMongoEntity mongoEntity)
        {
            comment.Id.Should().Be(mongoEntity.Id);
            comment.Content.Should().Be(mongoEntity.Content);
            comment.PostId.Should().Be(mongoEntity.PostId);
            comment.AssertAuditingProperties(mongoEntity);
        }
    }
}

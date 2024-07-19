using FluentAssertions;
using Vogel.Application.Comments.Commands;
using Vogel.Application.Comments.Dtos;
using Vogel.Domain.Comments;
using Vogel.Domain.Users;
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

        public static void AssertCommentDto(this CommentAggregateDto dto , Comment comment , UserAggregate? user = null)
        {
            dto.Id.Should().Be(comment.Id);
            dto.Content.Should().Be(comment.Content);
            dto.PostId.Should().Be(comment.PostId);
            dto.UserId.Should().Be(comment.UserId);
           
            if(user != null)
            {
                dto.User.AssertPublicUserDto(user);
            }
        }
    }
}

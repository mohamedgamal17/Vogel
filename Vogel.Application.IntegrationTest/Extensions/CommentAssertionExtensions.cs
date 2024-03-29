using FluentAssertions;
using Vogel.Application.Comments.Commands;
using Vogel.Domain;

namespace Vogel.Application.IntegrationTest.Extensions
{
    public static class CommentAssertionExtensions
    {

        public static void AssertComment(this Comment comment , CommentCommandBase command)
        {
            comment.PostId.Should().Be(comment.PostId);
            comment.Content.Should().Be(comment.Content);
        }
    }
}

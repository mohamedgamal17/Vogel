using FluentAssertions;
using Vogel.Application.Comments.Commands;
using Vogel.Domain;

namespace Vogel.Application.IntegrationTest.Extensions
{
    public static class CommentAssertionExtensions
    {

        public static void AssertComment(this Comment comment , CommentCommandBase command)
        {
            comment.Content.Should().Be(command.Content);
        }
    }
}

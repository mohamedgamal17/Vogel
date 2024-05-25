using FluentAssertions;
using System.Security.Claims;
using Vogel.Application.Posts.Commands;
using Vogel.Domain.Posts;
using static Vogel.Application.IntegrationTest.Testing;
namespace Vogel.Application.IntegrationTest.Extensions
{
    public static class PostAssertionExtenions
    {
        public static void AssertPost(this Post post, PostCommandBase command)
        {
            string userId = CurrentUser!.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
            post.Caption.Should().Be(command.Caption);
            post.MediaId.Should().Be(command.MediaId);
            post.UserId.Should().Be(userId);
        }
    }
}

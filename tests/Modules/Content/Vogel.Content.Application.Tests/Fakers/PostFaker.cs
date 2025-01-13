using Bogus;
using Vogel.Content.Domain.Posts;
namespace Vogel.Content.Application.Tests.Fakers
{
    public class PostFaker : Faker<Post>
    {
        public PostFaker(string userId , string? mediaId = null)
        {
            RuleFor(x => x.Caption, _ => Guid.NewGuid().ToString());
            RuleFor(x => x.UserId, userId);
            RuleFor(x => x.MediaId, mediaId);
        }
    }
}

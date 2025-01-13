using Bogus;
using Vogel.Content.Domain.Common;
using Vogel.Content.Domain.Posts;

namespace Vogel.Content.Application.Tests.Fakers
{
    public class PostReactionFaker : Faker<PostReaction>
    {
        public PostReactionFaker(string userId , string postId)
        {
            RuleFor(x => x.PostId, postId);
            RuleFor(x => x.UserId, userId);
            RuleFor(x => x.Type, f => f.PickRandom<ReactionType>());
        }
    }
}

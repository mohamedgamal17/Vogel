using Bogus;
using Vogel.Content.Domain.Comments;
using Vogel.Content.Domain.Common;

namespace Vogel.Content.Application.Tests.Fakers
{
    public class CommentReactionFaker : Faker<CommentReaction> 
    {
        public CommentReactionFaker(string userId , string commentId)
        {
            RuleFor(x => x.UserId, userId);
            RuleFor(x => x.CommentId, commentId);
            RuleFor(x => x.Type, f => f.PickRandom<ReactionType>());

        }
    }
}

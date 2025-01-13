using Bogus;
using Vogel.Content.Domain.Comments;

namespace Vogel.Content.Application.Tests.Fakers
{
    public class CommentFaker : Faker<Comment>

    {
        public CommentFaker(string userId, Comment comment)
            : this(userId, comment.PostId, comment.Id)
        {



        }

        public CommentFaker(List<string> usersId , Comment comment)
            : this(usersId, comment.PostId, comment.Id)
        {

        }
        public CommentFaker(List<string> usersId, string postId, string? parentId = null)
        {
            RuleFor(x => x.Content, f => f.Lorem.Sentence(5));
            RuleFor(x => x.UserId, f=> f.PickRandom(usersId));
            RuleFor(x => x.PostId, postId);
            RuleFor(x => x.CommentId, parentId);
        }



        public CommentFaker(string userId, string postId, string? parentId = null)
        {
            RuleFor(x => x.Content, f => f.Lorem.Sentence(5));
            RuleFor(x => x.UserId, userId);
            RuleFor(x => x.PostId, postId);
            RuleFor(x => x.CommentId, parentId);
        }
    }
}

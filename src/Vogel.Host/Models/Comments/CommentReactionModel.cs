using Vogel.Application.CommentReactions.Commands;
using Vogel.Domain.Posts;

namespace Vogel.Host.Models.Comments
{
    public class CommentReactionModel
    {
        public ReactionType Type { get; set; }

        public CreateCommentReactionCommand ToCreateCommentReactionCommand(string postId , string commentId)
        {
            return new CreateCommentReactionCommand
            {
                PostId = postId,
                CommentId = commentId,
                Type = Type
            };
        }

        public UpdateCommentReactionCommand ToUpdateCommentReactionCommand(string postId , string commentId , string reactionId)
        {
            return new UpdateCommentReactionCommand
            {
                PostId = postId,
                CommentId = commentId,
                ReactionId = reactionId,
                Type = Type
            };
        }
    }
}

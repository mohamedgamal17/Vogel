using Vogel.Application.Comments.Commands;

namespace Vogel.Host.Models.Comments
{

    public class CreateCommentModel
    {
        public string Content { get; set; }
        public string? CommentId { get; set; }

        public CreateCommentCommand ToCreateCommentCommand(string postId)
        {
            var command = new CreateCommentCommand
            {
                PostId = postId,
                Content = Content,
                CommentId = CommentId
            };
            return command;
        }
    }
    public class UpdateCommentModel
    {
        public string Content { get; set; }

        public UpdateCommentCommand ToUpdateCommentCommand(string postId, string commentId)
        {
            var command = new UpdateCommentCommand
            {
                PostId = postId,
                CommentId = commentId,
                Content = Content,
            };

            return command;
        }
    }
}

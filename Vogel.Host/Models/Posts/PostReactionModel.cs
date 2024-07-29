using Vogel.Application.PostReactions.Commands;
using Vogel.Domain.Posts;

namespace Vogel.Host.Models.Posts
{
    public class PostReactionModel
    {
        public ReactionType Type { get; set; }


        public CreatePostReactionCommand ToCreatePostReactionCommand(string postId)
        {
            var command = new CreatePostReactionCommand { PostId = postId };

            PreparePostReactionCommand(command);

            return command;
        }

        public UpdatePostReactionCommand ToUpdatePostReactionCommand(string postId , string reactionId)
        {
            var command = new UpdatePostReactionCommand
            {
                ReactionId = reactionId,
                PostId = postId
            };

            PreparePostReactionCommand(command);

            return command;
        }

        private void PreparePostReactionCommand(PostReactionCommandBase command)
        {
            command.Type = Type;
        }
    }
}

using Vogel.Application.Comments.Commands;
using Vogel.Application.Posts.Commands;

namespace Vogel.Host.Models
{
    public class PostModel
    {
        public string Caption { get; set; }
        public string MediaId { get; set; }

        public CreatePostCommand ToCreatePostCommand()
        {
            var command = new CreatePostCommand();

            PreparePostCommand(command);

            return command;

        }

        public UpdatePostCommand ToUpdatePostCommand(string postId)
        {
            var command = new UpdatePostCommand() {  Id = postId};

            PreparePostCommand(command);

            return command;

        }


        private void PreparePostCommand(PostCommandBase command)
        {
            command.Caption = Caption;
            command.MediaId = MediaId;
        }
    }
}

using MediatR;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.PostReactions.Dtos;
using Vogel.Domain.Posts;

namespace Vogel.Application.PostReactions.Commands
{
    public class PostReactionCommandBase
    {
        public string PostId { get; set; }
        public ReactionType Type { get; set; }
    }

    public class CreatePostReactionCommand : PostReactionCommandBase , ICommand<PostReactionDto>
    {
    }

    public class UpdatePostReactionCommand : PostReactionCommandBase  , ICommand<PostReactionDto>
    {
        public string Id { get; set; }
    }

    public class RemovePostReactionCommand: PostReactionCommandBase , ICommand<Unit>
    {
        public string Id { get; set; }
    }

}

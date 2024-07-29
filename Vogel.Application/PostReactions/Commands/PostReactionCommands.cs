using MediatR;
using Microsoft.AspNetCore.Authorization;
using Vogel.Application.PostReactions.Dtos;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Domain.Posts;
namespace Vogel.Application.PostReactions.Commands
{
    public class PostReactionCommandBase
    {
        public string PostId { get;  set; }
        public ReactionType Type { get; set; }

    }

    [Authorize]
    public class CreatePostReactionCommand : PostReactionCommandBase , ICommand<PostReactionDto>
    {

    }

    [Authorize]
    public class UpdatePostReactionCommand : PostReactionCommandBase  , ICommand<PostReactionDto>
    {
        public string ReactionId { get;  set; }
    }

    [Authorize]
    public class RemovePostReactionCommand:  ICommand<Unit>
    {
        public string ReactionId { get; set; }
        public string PostId { get;  set; }
    }

}

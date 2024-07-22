using MediatR;
using Microsoft.AspNetCore.Authorization;
using Vogel.Application.PostReactions.Dtos;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Domain.Posts;
namespace Vogel.Application.PostReactions.Commands
{
    public class PostReactionCommandBase
    {
        public string PostId { get; private set; }
        public ReactionType Type { get; set; }

        public void SetPostId(string postId)
        {
            PostId = postId;
        }
    }

    [Authorize]
    public class CreatePostReactionCommand : PostReactionCommandBase , ICommand<PostReactionDto>
    {
    }

    [Authorize]
    public class UpdatePostReactionCommand : PostReactionCommandBase  , ICommand<PostReactionDto>
    {
        public string Id { get; private set; }

        public void SetId(string id)
        {
            Id = id;
        }
    }

    [Authorize]
    public class RemovePostReactionCommand:  ICommand<Unit>
    {
        public string Id { get; set; }
        public string PostId { get;  set; }
    }

}

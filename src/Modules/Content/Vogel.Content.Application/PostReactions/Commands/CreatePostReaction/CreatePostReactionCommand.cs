using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.Content.Domain.Common;
namespace Vogel.Content.Application.PostReactions.Commands.CreatePostReaction
{
    [Authorize]
    public class CreatePostReactionCommand : ICommand<PostReactionDto>
    {
        public string PostId { get; set; }
        public ReactionType Type { get; set; }
    }
}

using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.Content.Domain.Common;
namespace Vogel.Content.Application.PostReactions.Commands.UpdatePostReaction
{
    public class UpdatePostReactionCommand : ICommand<PostReactionDto>
    {
        public string ReactionId { get; set; }
        public string PostId { get; set; }
        public ReactionType Type { get; set; }
    }
}

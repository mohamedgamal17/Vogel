using MediatR;
using Vogel.BuildingBlocks.Application.Requests;

namespace Vogel.Content.Application.PostReactions.Commands.RemovePostReaction
{
    public class RemovePostReactionCommand : ICommand<Unit>
    {
        public string ReactionId { get; set; }
        public string PostId { get; set; }
    }
}

using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;

namespace Vogel.Messanger.Application.Conversations.Commands.MarkConversationAsSeen
{
    public class MarkConversationAsSeenCommand : ICommand
    {
        public string ConversationId { get; set; }
        public string UserId { get; set; }
    }
}

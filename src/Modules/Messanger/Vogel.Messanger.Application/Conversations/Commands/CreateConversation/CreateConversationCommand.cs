using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Messanger.Application.Conversations.Dtos;

namespace Vogel.Messanger.Application.Conversations.Commands.CreateConversation
{
    [Authorize]
    public class CreateConversationCommand : ICommand<ConversationDto>
    {
        public string? Name { get; set; }
        public List<string> Participants { get; set; }
    }
}

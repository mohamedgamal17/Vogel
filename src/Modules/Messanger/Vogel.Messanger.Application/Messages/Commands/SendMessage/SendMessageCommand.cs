using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Messanger.Application.Messages.Dtos;
namespace Vogel.Messanger.Application.Messages.Commands.SendMessage
{
    [Authorize]
    public class SendMessageCommand : ICommand<MessageDto>
    {
        public string ConversationId { get; set; }
        public string Content { get; set; }

    }
}

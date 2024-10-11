using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Messanger.Application.Messages.Dtos;
namespace Vogel.Messanger.Application.Messages.Commands.MarkMessageAsSeen
{
    [Authorize]
    public class MarkMessageAsSeenCommand : ICommand<MessageDto>
    {
        public string MessageId { get; set; }
    }
}

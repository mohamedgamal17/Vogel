using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Messanger.Application.Messages.Dtos;
namespace Vogel.Messanger.Application.Messages.Commands
{
    public class SendMessageCommand : ICommand<MessageDto>
    {
        public string Content { get; set; }
        public string ReciverId { get; set; }
    }
}

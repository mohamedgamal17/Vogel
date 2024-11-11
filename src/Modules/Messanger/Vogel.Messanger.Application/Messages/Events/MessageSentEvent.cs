using Vogel.Messanger.Domain.Messages;

namespace Vogel.Messanger.Application.Messages.Events
{
    public class MessageSentEvent
    {
        public Message Message { get; set; }
    }
}

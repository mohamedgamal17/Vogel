using Vogel.Messanger.Domain.Messages;

namespace Vogel.Messanger.Application.Messages.Events
{
    public class LogMessagesActivitiesEvent
    {
        public List<MessageActivity> Activities { get; set; } = new List<MessageActivity>();
        public string SeenById { get; set; }
    }
}

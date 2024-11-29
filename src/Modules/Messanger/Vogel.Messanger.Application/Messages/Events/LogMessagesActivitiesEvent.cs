using Vogel.Messanger.Domain.Messages;

namespace Vogel.Messanger.Application.Messages.Events
{
    public class LogMessagesActivitiesEvent
    {
        public string ConversationId { get; set; }
        public string SeenById { get; set; }
        public DateTime SeentAt { get; set; }
        public LogMessagesActivitiesEvent(string conversationId, string seenById, DateTime seentAt)
        {
            ConversationId = conversationId;
            SeenById = seenById;
            SeentAt = seentAt;
        }
    }
}

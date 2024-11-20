namespace Vogel.Messanger.Application.Conversations.Events
{
    public class MarkConversationAsSeenEvent
    {
        public string ConversationId { get; set; }
        public string UserId { get; set; }
    }
}

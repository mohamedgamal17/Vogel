using Vogel.BuildingBlocks.Domain.Auditing;
namespace Vogel.Messanger.Domain.Messages
{
    public class Message : AuditedEntity<string>
    {
        public string Content { get; set; }
        public string ConversationId { get; set; }
        public string SenderId { get; set; }
        public Message()
        {
            Id = Guid.NewGuid().ToString();
        }

    }
}

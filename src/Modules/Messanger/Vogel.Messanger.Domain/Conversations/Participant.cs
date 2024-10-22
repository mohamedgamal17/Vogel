using Vogel.BuildingBlocks.Domain.Auditing;

namespace Vogel.Messanger.Domain.Conversations
{
    public class Participant : AuditedEntity<string>
    {
        public string UserId { get; set; }
        public string ConversationId { get; set; }

        public Participant()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}

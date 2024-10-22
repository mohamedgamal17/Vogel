using Vogel.BuildingBlocks.Domain.Auditing;

namespace Vogel.Messanger.Domain.Conversations
{
    public class Conversation : AuditedAggregateRoot<string>
    {
        public string? Name { get; set; }

        public Conversation()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}

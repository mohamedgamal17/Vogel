using Vogel.BuildingBlocks.Domain.Auditing;
namespace Vogel.Messanger.Domain.Messages
{
    public class MessageActivity : AuditedEntity<string>
    {
        public string MessageId { get; set; }
        public string SeenById { get; set; }
        public DateTime SeenAt { get; set; }

        public MessageActivity()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}

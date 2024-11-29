using Vogel.BuildingBlocks.Domain.Auditing;
namespace Vogel.Messanger.Domain.Messages
{
    public class MessageLog : AuditedEntity<string>
    {
        public string MessageId { get; set; }
        public string SeenById { get; set; }
        public DateTime SeenAt { get; set; }

        public MessageLog()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}

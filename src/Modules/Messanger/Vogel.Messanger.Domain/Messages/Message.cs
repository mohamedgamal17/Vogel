using Vogel.BuildingBlocks.Domain.Auditing;
namespace Vogel.Messanger.Domain.Messages
{
    public class Message : AuditedEntity<string>
    {
        public string Content { get; set; }
        public string SenderId { get; set; }
        public string ReciverId { get; set; }
        public bool IsSeen { get; set; }
        public Message()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}

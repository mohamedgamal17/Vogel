using Vogel.BuildingBlocks.Domain.Auditing;

namespace Vogel.Domain.Friendship
{
    public class Friend : AuditedAggregateRoot<string>
    {
        public string SourceId { get; set; }
        public string TargetId { get; set; }

        public Friend()
        {
            Id = Guid.NewGuid().ToString();
        }

    }
}

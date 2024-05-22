
namespace Vogel.BuildingBlocks.Domain.Auditing
{
    public class AuditedAggregateRoot<TKey> : AggregateRoot<TKey>, IAuditedEntity
    {
        public string? CreatorId { get; set; }
        public DateTimeOffset CreationTime { get; set; }
        public DateTimeOffset? ModificationTime { get; set; }
        public string? ModifierId { get; set; }
        public DateTimeOffset? DeletionTime { get; set; }
        public string? DeletorId { get; set; }
    }
}

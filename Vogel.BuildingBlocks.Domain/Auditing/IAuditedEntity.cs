namespace Vogel.BuildingBlocks.Domain.Auditing
{
    public interface IAuditedEntity
    {
        public string? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? ModificationTime { get; set; }
        public string? ModifierId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string? DeletorId { get; set; }
    }
}

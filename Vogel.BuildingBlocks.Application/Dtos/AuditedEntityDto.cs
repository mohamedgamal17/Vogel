namespace Vogel.BuildingBlocks.Application.Dtos
{
    public abstract class AuditedEntityDto<T> : EntityDto<T> , IAuditedEntityDto<T>
    {
        public string? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? ModificationTime { get; set; }
        public string? ModifierId { get; set; }
        public DateTime? DeletionTime { get; set; }
        public string? DeletorId { get; set; }
    }
}

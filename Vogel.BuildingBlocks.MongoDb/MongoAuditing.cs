namespace Vogel.BuildingBlocks.MongoDb
{
    public interface IMongoAuditing
    {
         string? CreatorId { get; set; }
         DateTimeOffset CreationTime { get; set; }
         DateTimeOffset? ModificationTime { get; set; }
         string? ModifierId { get; set; }
         DateTimeOffset? DeletionTime { get; set; }
         string? DeletorId { get; set; }
    }

    public class FullAuditedMongoEntity<TKey> :MongoEntity<TKey> , IMongoAuditing
    {
        public string? CreatorId { get; set; }
        public DateTimeOffset CreationTime { get; set; }
        public DateTimeOffset? ModificationTime { get; set; }
        public string? ModifierId { get; set; }
        public DateTimeOffset? DeletionTime { get; set; }
        public string? DeletorId { get; set; }

    }
}

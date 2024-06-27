using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Vogel.BuildingBlocks.MongoDb
{
    public interface IMongoAuditing
    {
        string? CreatorId { get; set; }
        DateTime CreationTime { get; set; }
        DateTime ModificationTime { get; set; }
        string? ModifierId { get; set; }
        DateTime? DeletionTime { get; set; }
        string? DeletorId { get; set; }
    }

    public class FullAuditedMongoEntity<TKey> : MongoEntity<TKey>, IMongoAuditing
    {
        public string? CreatorId { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, Representation = BsonType.DateTime)]
        public DateTime CreationTime { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime ModificationTime { get; set; }
        public string? ModifierId { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? DeletionTime { get; set; }
        public string? DeletorId { get; set; }

    }
}

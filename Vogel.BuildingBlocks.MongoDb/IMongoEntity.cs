using MongoDB.Bson.Serialization.Attributes;

namespace Vogel.BuildingBlocks.MongoDb
{
    public interface IMongoEntity
    {

    }

    public interface IMongoEntity<TKey> : IMongoEntity
    {
        TKey Id { get; set; }
    }
    public abstract class MongoEntity<TKey> : IMongoEntity<TKey>
    {
        [BsonId]
        [BsonElement("_id")]
        public TKey Id { get; set; }
    }

}

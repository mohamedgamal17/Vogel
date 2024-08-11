using MongoDB.Bson.Serialization.Attributes;

namespace Vogel.BuildingBlocks.MongoDb
{
    public interface IMongoEntity
    {
        string Id { get; set; }
    }

    public abstract class MongoEntity : IMongoEntity
    {
        [BsonId]
        [BsonElement("_id")]
        public string Id { get; set; }
    }

}

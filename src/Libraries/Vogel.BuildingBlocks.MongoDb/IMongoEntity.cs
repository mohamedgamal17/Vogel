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

    public abstract class MongoOwnedEntity : MongoEntity, IMongoOwnedEntity
    {
        public string UserId { get; set; }

        public bool IsOwnedBy(string userId)
        {
            return userId == UserId;
        }
    }
}

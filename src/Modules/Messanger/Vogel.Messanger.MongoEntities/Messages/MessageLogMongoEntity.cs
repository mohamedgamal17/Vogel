using MongoDB.Bson.Serialization.Attributes;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Messanger.MongoEntities.Users;

namespace Vogel.Messanger.MongoEntities.Messages
{
    public class MessageLogMongoEntity : FullAuditedMongoEntity<string>
    {
        public string SeenById { get; set; }
        public DateTime SeenAt { get; set; }

        [BsonIgnoreIfNull]
        public UserMongoEntity User { get; set; }
    }
}

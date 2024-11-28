using MongoDB.Bson.Serialization.Attributes;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Messanger.MongoEntities.Users;
namespace Vogel.Messanger.MongoEntities.Conversations
{
    public class ParticipantMongoEntity : FullAuditedMongoEntity<string>
    {
        public string UserId { get; set; }
        public string ConversationId { get; set; }

        [BsonIgnoreIfNull]
        public UserMongoEntity User { get; set; }
    }
}

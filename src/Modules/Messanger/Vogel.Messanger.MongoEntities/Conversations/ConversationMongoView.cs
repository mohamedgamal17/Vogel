using MongoDB.Bson.Serialization.Attributes;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Messanger.MongoEntities.Messages;

namespace Vogel.Messanger.MongoEntities.Conversations
{
    public class ConversationMongoView : FullAuditedMongoEntity<string>
    {
        public string? Name { get; set; }
        public Paging<ParticipantMongoView> Participants { get; set; }

        public Paging<MessageMongoView> Messages { get; set; }
    }

    public class ConversationQueryMongoView : FullAuditedMongoEntity<string>
    {
        public string? Name { get; set; }

        [BsonElement("_participants")]
        public List<ParticipantMongoView> Participants { get; set; }

        [BsonElement("_messages")]
        public List<MessageMongoView> Messages { get; set; }
    } 

}

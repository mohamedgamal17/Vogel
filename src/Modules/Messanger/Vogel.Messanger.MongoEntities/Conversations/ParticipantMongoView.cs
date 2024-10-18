
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Messanger.MongoEntities.Users;

namespace Vogel.Messanger.MongoEntities.Conversations
{
    public class ParticipantMongoView : FullAuditedMongoEntity<string>
    {
        public string UserId { get; set; }
        public string ConversationId { get; set; }
        public UserMongoEntity User { get; set; }
    }
}

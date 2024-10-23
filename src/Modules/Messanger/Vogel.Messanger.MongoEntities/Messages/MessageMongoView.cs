using Vogel.BuildingBlocks.MongoDb;
using Vogel.Messanger.MongoEntities.Users;

namespace Vogel.Messanger.MongoEntities.Messages
{
    public class MessageMongoView : FullAuditedMongoEntity<string>
    {
        public string Content { get; set; }
        public string ConversationId { get; set; }
        public string SenderId { get; set; }
        public UserMongoEntity Sender { get; set; }
    }
}

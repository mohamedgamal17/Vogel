using Vogel.BuildingBlocks.MongoDb;
using Vogel.Messanger.MongoEntities.Conversations;
using Vogel.Messanger.MongoEntities.Users;

namespace Vogel.Messanger.MongoEntities.Messages
{
    public class MessageMongoView : FullAuditedMongoEntity<string>
    {
        public string Content { get; set; }
        public string ConversationId { get; set; }
        public string SenderId { get; set; }
        public UserMongoEntity Sender { get; set; }
        public bool IsSeen { get; set; }
    }

    internal class MessageJoinedView : FullAuditedMongoEntity<string>
    {
        public string Content { get; set; }
        public string ConversationId { get; set; }
        public string SenderId { get; set; }
        public List<MessageLogMongoEntity> Logs { get; set; }
        public ConversationMongoEntity Conversation{get; set;}
        public UserMongoEntity Sender { get; set; }
    }

}

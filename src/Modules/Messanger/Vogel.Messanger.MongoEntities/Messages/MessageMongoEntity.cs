using Vogel.BuildingBlocks.MongoDb;
using Vogel.Messanger.MongoEntities.Conversations;
namespace Vogel.Messanger.MongoEntities.Messages
{
    [MongoCollection(MessageMongoConsts.MessageCollection)]
    public class MessageMongoEntity : FullAuditedMongoEntity<string>
    {
        public string Content { get; set; }
        public string ConversationId { get; set; }
        public string SenderId { get; set; }
    }
}

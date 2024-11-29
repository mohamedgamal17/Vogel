using Vogel.BuildingBlocks.MongoDb;
namespace Vogel.Messanger.MongoEntities.Messages
{
    [MongoCollection(MessageMongoConsts.MessageCollection)]
    public class MessageMongoEntity : FullAuditedMongoEntity<string>
    {
        public string Content { get; set; }
        public string ConversationId { get; set; }
        public string SenderId { get; set; }
        public List<MessageLogMongoEntity> Logs { get; set; }

        public MessageMongoEntity()
        {
            Logs = new List<MessageLogMongoEntity>();
        }

    }
}

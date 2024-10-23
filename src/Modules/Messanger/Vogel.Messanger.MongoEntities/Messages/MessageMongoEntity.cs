using Vogel.BuildingBlocks.MongoDb;
namespace Vogel.Messanger.MongoEntities.Messages
{

    public class MessageMongoEntity : FullAuditedMongoEntity<string>
    {
        public string Content { get; set; }
        public string ConversationId { get; set; }
        public string SenderId { get; set; }
    }
}

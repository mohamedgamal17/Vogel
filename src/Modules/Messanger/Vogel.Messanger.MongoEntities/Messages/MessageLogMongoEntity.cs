using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.Messanger.MongoEntities.Messages
{
    public class MessageLogMongoEntity : FullAuditedMongoEntity<string>
    {
        public string MessageId { get; set; }
        public string SeenById { get; set; }
        public DateTime SeenAt { get; set; }

    }
}

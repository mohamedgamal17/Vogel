using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.Messanger.MongoEntities.Messages
{
    [MongoCollection(MessageMongoConsts.MessageActivityCollection)]
    public class MessageActivityMongoEntity : FullAuditedMongoEntity<string>
    {
        public string MessageId { get; set; }
        public string SeenById { get; set; }
        public DateTime SeenAt { get; set; }

    }
}

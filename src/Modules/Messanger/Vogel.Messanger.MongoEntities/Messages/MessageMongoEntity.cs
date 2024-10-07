using Vogel.BuildingBlocks.MongoDb;
namespace Vogel.Messanger.MongoEntities.Messages
{

    public class MessageMongoEntity : FullAuditedMongoEntity<string>
    {
        public string Content { get; set; }
        public string SenderId { get; set; }
        public string ReciverId { get; set; }
        public bool IsSeen { get; set; }
    }
}

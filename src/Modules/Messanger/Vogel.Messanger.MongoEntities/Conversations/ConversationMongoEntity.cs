using Vogel.BuildingBlocks.MongoDb;
namespace Vogel.Messanger.MongoEntities.Conversations
{
    public class ConversationMongoEntity : FullAuditedMongoEntity<string>
    {
        public string? Name { get; set; }
    }
}

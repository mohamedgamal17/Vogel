using Vogel.BuildingBlocks.MongoDb;
namespace Vogel.Messanger.MongoEntities.Conversations
{
    [MongoCollection(ConversationConsts.ConversationCollection)]
    public class ConversationMongoEntity : FullAuditedMongoEntity<string>
    {
        public string? Name { get; set; }
    }
}

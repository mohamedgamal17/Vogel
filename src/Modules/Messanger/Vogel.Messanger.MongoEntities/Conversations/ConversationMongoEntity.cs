using Vogel.BuildingBlocks.MongoDb;
namespace Vogel.Messanger.MongoEntities.Conversations
{
    [MongoCollection(ConversationConsts.ConversationCollection)]
    public class ConversationMongoEntity : FullAuditedMongoEntity<string>
    {
        public string? Name { get; set; }
        public List<ParticipantMongoEntity> Participants { get; set; }
        public ConversationMongoEntity()
        {
            Participants = new List<ParticipantMongoEntity>();
        }
    }
}

using Vogel.BuildingBlocks.MongoDb;
namespace Vogel.Messanger.MongoEntities.Conversations
{
    [MongoCollection(ConversationConsts.ParticipantCollection)]
    public class ParticipantMongoEntity : FullAuditedMongoEntity<string>
    {
        public string UserId { get; set; }
        public string ConversationId { get; set; }
    }
}

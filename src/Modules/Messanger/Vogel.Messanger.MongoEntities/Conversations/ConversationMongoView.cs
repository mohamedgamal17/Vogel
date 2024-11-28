using Vogel.BuildingBlocks.MongoDb;
namespace Vogel.Messanger.MongoEntities.Conversations
{
    public class ConversationMongoView : FullAuditedMongoEntity<string>
    {
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public int TotalParticpants { get; set; }
        public List<ParticipantMongoEntity> Participants{get; set;}
    }

    internal class ConversationJoinedView : FullAuditedMongoEntity<string>
    {
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public ParticipantMongoEntity Participants { get; set; }
    }

}

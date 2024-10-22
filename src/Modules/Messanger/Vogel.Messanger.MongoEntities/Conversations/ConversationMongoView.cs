using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.Messanger.MongoEntities.Conversations
{
    public class ConversationMongoView : FullAuditedMongoEntity<string>
    {
        public string? Name { get; set; }
        public List<ParticipantMongoView> Participants { get; set; }
    }

    internal class ConversationUngroupedMongoView : FullAuditedMongoEntity<string>
    {
        public string? Name { get; set; }
        public ParticipantMongoView Participant { get; set; }
    }

}

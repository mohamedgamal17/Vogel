using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.MongoEntities.PostReactions;

namespace Vogel.Content.MongoEntities.CommentReactions
{

    [MongoCollection(CommentReactionMongoConsts.CollectionName)]
    public class CommentReactionMongoEntity : FullAuditedMongoEntity<string>
    {
        public string UserId { get; set; }

        public string CommentId { get; set; }

        public ReactionType Type { get; set; }
    }
}

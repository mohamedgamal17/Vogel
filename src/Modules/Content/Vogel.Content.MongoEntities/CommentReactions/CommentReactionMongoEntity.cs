using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.MongoEntities.PostReactions;

namespace Vogel.Content.MongoEntities.CommentReactions
{

    [MongoCollection(CommentReactionMongoConsts.CollectionName)]
    public class CommentReactionMongoEntity : OwnedFullAuditedMongoEntity<string>
    {
        public string CommentId { get; set; }
        public ReactionType Type { get; set; }
    }
}

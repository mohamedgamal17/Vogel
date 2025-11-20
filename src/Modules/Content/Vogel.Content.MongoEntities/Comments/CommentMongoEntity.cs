using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.Content.MongoEntities.Comments
{
    [MongoCollection(CommentMongoConsts.CollectioName)]
    public class CommentMongoEntity : OwnedFullAuditedMongoEntity<string>
    {
        public string Content { get; set; }
        public string PostId { get; set; }
        public string? CommentId { get; set; }
    }
}

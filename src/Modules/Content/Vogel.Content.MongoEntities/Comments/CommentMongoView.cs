using Vogel.BuildingBlocks.MongoDb;
namespace Vogel.Content.MongoEntities.Comments
{
    public class CommentMongoView : OwnedFullAuditedMongoEntity<string>
    {
        public string Content { get; set; }
        public string PostId { get; set; }
        public string? CommentId { get; set; }
    }
}

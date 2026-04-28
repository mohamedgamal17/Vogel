using Vogel.BuildingBlocks.MongoDb;
namespace Vogel.Content.MongoEntities.Posts
{
    public class PostMongoView : FullAuditedMongoEntity<string>
    {
        public string Caption { get; set; }
        public string? MediaId { get; set; }
        public string UserId { get; set; }
    }
}

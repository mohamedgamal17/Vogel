using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.Content.MongoEntities.Posts
{
    [MongoCollection(PostMongoConsts.CollectionName)]
    public class PostMongoEntity : FullAuditedMongoEntity<string>
    {
        public string Caption { get; set; }
        public string? MediaId { get; set; }
        public string UserId { get; set; }
    }
}

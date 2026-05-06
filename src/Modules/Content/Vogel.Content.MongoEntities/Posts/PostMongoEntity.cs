using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.Content.MongoEntities.Posts
{
    [MongoCollection(PostMongoConsts.CollectionName)]
    public class PostMongoEntity : OwnedFullAuditedMongoEntity<string>
    {
        public string Caption { get; set; }
        public string? MediaId { get; set; }
    }
}

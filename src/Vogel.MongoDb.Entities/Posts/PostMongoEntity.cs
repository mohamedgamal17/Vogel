using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.Posts
{
    [MongoCollection(PostMongoConsts.CollectionName)]
    public class PostMongoEntity : FullAuditedMongoEntity<string>
    {
        public string Caption { get; set; }
        public string? MediaId { get; set; }
        public string UserId { get; set; }
    }
}

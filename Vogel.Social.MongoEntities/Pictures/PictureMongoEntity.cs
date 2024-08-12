using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.Social.MongoEntities.Pictures
{
    [MongoCollection(PictureMongoConsts.CollectionName)]
    public class PictureMongoEntity : FullAuditedMongoEntity<string>
    {
        public string File { get; set; }
        public string UserId { get; set; }
    }
}

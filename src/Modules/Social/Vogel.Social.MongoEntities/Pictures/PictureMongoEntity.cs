using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.Social.MongoEntities.Pictures
{
    [MongoCollection(PictureMongoConsts.CollectionName)]
    public class PictureMongoEntity : OwnedFullAuditedMongoEntity
    {
        public string File { get; set; }
    }
}

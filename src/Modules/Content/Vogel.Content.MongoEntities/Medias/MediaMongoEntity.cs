using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.Content.MongoEntities.Medias
{
    [MongoCollection(MediaMongoConsts.CollectionName)]
    public class MediaMongoEntity : OwnedFullAuditedMongoEntity<string>
    {
        public string File { get; set; }
        public MediaType MediaType { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
    }
}

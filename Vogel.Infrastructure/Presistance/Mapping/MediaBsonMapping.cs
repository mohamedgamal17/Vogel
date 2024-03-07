using MongoDB.Bson.Serialization;
using Vogel.Domain;

namespace Vogel.Infrastructure.Presistance.Mapping
{
    public class MediaBsonMapping : IMongoDbClassMap<Media>
    {
        public void Map(BsonClassMap<Media> cm)
        {
            cm.AutoMap();     
        }
    }
}

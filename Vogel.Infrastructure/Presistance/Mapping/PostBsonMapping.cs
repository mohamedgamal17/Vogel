using MongoDB.Bson.Serialization;
using Vogel.Domain;
using Vogel.Infrastructure.Presistance;

namespace Vogel.Infrastructure.Presistance.Mapping
{
    public class PostBsonMapping : IMongoDbClassMap<Post>
    {
        public void Map(BsonClassMap<Post> cm)
        {
            cm.AutoMap();

            cm.MapMember(x => x.Caption);

            cm.MapMember(x => x.MediaId);

            cm.MapMember(x => x.UserId);
        }
    }
}

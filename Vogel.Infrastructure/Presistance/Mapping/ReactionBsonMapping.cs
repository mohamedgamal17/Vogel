using MongoDB.Bson.Serialization;
using Vogel.Domain;

namespace Vogel.Infrastructure.Presistance.Mapping
{
    public class ReactionBsonMapping : IMongoDbClassMap<PostReaction>
    {
        public void Map(BsonClassMap<PostReaction> cm)
        {
            cm.AutoMap();
            cm.MapMember(x => x.PostId);
            cm.MapMember(x => x.UserId);
            cm.MapMember(x => x.Type);

        }
    }
}

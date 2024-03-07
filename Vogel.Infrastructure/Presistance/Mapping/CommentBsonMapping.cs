using MongoDB.Bson.Serialization;
using Vogel.Domain;
namespace Vogel.Infrastructure.Presistance.Mapping
{
    public class CommentBsonMapping : IMongoDbClassMap<Comment>
    {
        public void Map(BsonClassMap<Comment> cm)
        {
            cm.AutoMap();

            cm.MapMember(x => x.Content);
            cm.MapMember(x => x.UserId);
            cm.MapMember(x => x.PostId);
        }
    }
}

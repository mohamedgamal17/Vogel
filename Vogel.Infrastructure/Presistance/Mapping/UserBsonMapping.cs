using MongoDB.Bson.Serialization;
using Vogel.Domain;

namespace Vogel.Infrastructure.Presistance.Mapping
{
    public class UserBsonMapping : IMongoDbClassMap<User>
    {
        public void Map(BsonClassMap<User> cm)
        {
            cm.AutoMap();

            cm.MapMember(x => x.FirstName);

            cm.MapMember(x => x.LastName);

            cm.MapMember(x => x.BirthDate);

            cm.MapMember(x => x.AvatarId);


        }
    }
}

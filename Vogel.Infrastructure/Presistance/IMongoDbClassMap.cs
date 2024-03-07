using MongoDB.Bson.Serialization;
namespace Vogel.Infrastructure.Presistance
{
    public interface IMongoDbClassMap<T>
    {
        public void Map(BsonClassMap<T> cm);
    }
}

using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.Social.MongoEntities.Pictures
{
    public class PictureMongoRepository : MongoRepository<PictureMongoEntity>
    {
        public PictureMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }
    }
}

using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.Medias
{
    public class MediaMongoRepository : MongoRepository<MediaMongoEntity, string>
    {
        protected override string CollectionName => "medias";
        public MediaMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {


        }
    }
}

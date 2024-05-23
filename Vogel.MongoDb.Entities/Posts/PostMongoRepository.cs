using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.Posts
{
    public class PostMongoRepository : MongoRepository<PostMongoEntity, string>
    {
        protected override string CollectionName => "posts";
        public PostMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }
    }
}

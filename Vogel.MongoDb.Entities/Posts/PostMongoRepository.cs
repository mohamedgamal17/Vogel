using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
namespace Vogel.MongoDb.Entities.Posts
{
    public class PostMongoRepository : MongoRepository<PostMongoEntity, string>
    {
        public PostMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }

        protected override string CollectionName => "posts";
    }
}

using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.Posts
{
    public class PostMongoViewRepository : MongoRepository<PostMongoView, string>
    {
        protected override string CollectionName => "posts_view";
        public PostMongoViewRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }
    }
}

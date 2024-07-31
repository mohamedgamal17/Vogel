using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.PostReactions
{
    public class PostReactionMongoViewRepository : MongoRepository<PostReactionMongoView, string>
    {
        public PostReactionMongoViewRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }

    }
}

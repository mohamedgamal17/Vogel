using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Posts;

namespace Vogel.MongoDb.Entities.PostReactions
{
    public class PostReactionMongoRepository : MongoRepository<PostReactionMongoEntity, string>
    {
        public PostReactionMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {

        }


   
    }
}

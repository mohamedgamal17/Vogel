using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Posts;

namespace Vogel.MongoDb.Entities.Reactions
{
    public class ReactionMongoRepository : MongoRepository<PostMongoEntity, string>
    {
        protected override string CollectionName => "reactions";
        public ReactionMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }
    }
}

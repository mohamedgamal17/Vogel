using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Posts;

namespace Vogel.MongoDb.Entities.PostReactions
{
    public class ReactionMongoRepository : MongoRepository<ReactionMongoEntity, string>
    {
        protected override string CollectionName => "reactions";
        public ReactionMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }
    }
}

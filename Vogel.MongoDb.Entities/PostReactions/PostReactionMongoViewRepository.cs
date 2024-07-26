using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.PostReactions
{
    public class PostReactionMongoViewRepository : MongoRepository<PostReactionMongoView, string>
    {
        protected override string CollectionName => "post_reactions_view";
        public PostReactionMongoViewRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }

    }
}

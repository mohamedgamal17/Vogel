using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.CommentReactions
{
    public class CommentReactionMongoViewRepository : MongoRepository<CommentReactionMongoView, string>
    {
        protected override string CollectionName => "comment_reactions_view";
        public CommentReactionMongoViewRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }
    }
}

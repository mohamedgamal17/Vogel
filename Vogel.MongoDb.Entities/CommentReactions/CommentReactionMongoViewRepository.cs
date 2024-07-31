using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.CommentReactions
{
    public class CommentReactionMongoViewRepository : MongoRepository<CommentReactionMongoView, string>
    {
        public CommentReactionMongoViewRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }
    }
}

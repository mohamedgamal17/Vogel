using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.CommentReactions
{
    public class CommentReactionMongoRepository : MongoRepository<CommentReactionMongoEntity, string>
    {
        public CommentReactionMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {

        }
    }
}

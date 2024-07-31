using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.Comments
{
    [MongoCollection("comments")]
    public class CommentMongoRepository : MongoRepository<CommentMongoEntity, string>
    {
        public CommentMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }
    }
}

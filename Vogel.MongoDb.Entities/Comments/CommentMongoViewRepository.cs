using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.Comments
{
    [MongoCollection("comments_view")]
    public class CommentMongoViewRepository : MongoRepository<CommentMongoView, string>
    {
        public CommentMongoViewRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }
    }
}

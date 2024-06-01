using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.Comments
{
    public class CommentMongoViewRepository : MongoRepository<CommentMongoView, string>
    {
        protected override string CollectionName => "comments_view";
        public CommentMongoViewRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }
    }
}

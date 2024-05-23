using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.Comments
{
    public class CommentMongoRepository : MongoRepository<CommentMongoEntity, string>
    {
        protected override string CollectionName => "comments";
        public CommentMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }
    }
}

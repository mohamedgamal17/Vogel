using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
namespace Vogel.Content.MongoEntities.Posts
{
    public class PostMongoRepository : MongoRepository<PostMongoEntity>
    {
        public PostMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }


        public async Task<PostMongoView?> GetPostViewById(string postId)
        {
            var query = PreparePostViewQuery()
                .Match(Builders<PostMongoView>.Filter.Eq(x => x.Id, postId));

            var view  = await query.SingleOrDefaultAsync();

            return view;
        }

        public IAggregateFluent<PostMongoView> PreparePostViewQuery()
        {
            return AsMongoCollection()
                .Aggregate()
                .As<PostMongoView>();
        }
    }
}

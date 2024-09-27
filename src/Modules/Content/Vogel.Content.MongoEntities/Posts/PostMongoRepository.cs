using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.MongoEntities.Medias;
using MongoDB.Driver.Linq;
namespace Vogel.Content.MongoEntities.Posts
{
    public class PostMongoRepository : MongoRepository<PostMongoEntity>
    {
        private readonly IMongoRepository<MediaMongoEntity> _mediaRepository;
        public PostMongoRepository(IMongoDatabase mongoDatabase, IMongoRepository<MediaMongoEntity> mediaRepository) : base(mongoDatabase)
        {
            _mediaRepository = mediaRepository;
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
                .Lookup<PostMongoEntity, MediaMongoEntity, PostMongoView>(
                    foreignCollection: _mediaRepository.AsMongoCollection(),
                    localField: l => l.MediaId,
                    foreignField: f => f.Id,
                    @as: r => r.Media
                )
                .Unwind<PostMongoView, PostMongoView>(x => x.Media, new AggregateUnwindOptions<PostMongoView> { PreserveNullAndEmptyArrays = true });
        }
    }
}

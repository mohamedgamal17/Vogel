using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.MongoEntities.Medias;
using Vogel.Content.MongoEntities.PostReactions;
using MongoDB.Driver.Linq;
namespace Vogel.Content.MongoEntities.Posts
{
    public class PostMongoRepository : MongoRepository<PostMongoEntity>
    {
        private readonly PostReactionMongoRepository _postReactionRepository;
        private readonly IMongoRepository<MediaMongoEntity> _mediaRepository;
        public PostMongoRepository(IMongoDatabase mongoDatabase, PostReactionMongoRepository postReactionRepository, IMongoRepository<MediaMongoEntity> mediaRepository) : base(mongoDatabase)
        {
            _postReactionRepository = postReactionRepository;
            _mediaRepository = mediaRepository;
        }

        public async Task<PostMongoView> GetPostViewById(string postId)
        {
            var query = PreparePostMongoViewQuery()
                .Match(Builders<PostMongoView>.Filter.Eq(x => x.Id, postId));

            var view  = await query.SingleOrDefaultAsync();

            var reactionSummary = await _postReactionRepository.GetPostReactionSummary(postId);

            view.ReactionSummary = reactionSummary;

            return view;
        }

        private IAggregateFluent<PostMongoView> PreparePostMongoViewQuery()
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

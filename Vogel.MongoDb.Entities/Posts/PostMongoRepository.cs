using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Common;
using Vogel.MongoDb.Entities.Extensions;
using Vogel.MongoDb.Entities.Medias;
using Vogel.MongoDb.Entities.PostReactions;
using Vogel.MongoDb.Entities.Users;
namespace Vogel.MongoDb.Entities.Posts
{
    public class PostMongoRepository : MongoRepository<PostMongoEntity>
    {
        private readonly UserMongoRepository _userMongoRepository;
        private readonly MediaMongoRepository _mediaMongoRepository;
        private readonly IMongoRepository<PostReactionSummaryMongoView> _postReactionSummaryRepository;

        public PostMongoRepository(IMongoDatabase mongoDatabase, UserMongoRepository userMongoRepository, MediaMongoRepository mediaMongoRepository, IMongoRepository<PostReactionSummaryMongoView> postReactionSummaryRepository) : base(mongoDatabase)
        {
            _userMongoRepository = userMongoRepository;
            _mediaMongoRepository = mediaMongoRepository;
            _postReactionSummaryRepository = postReactionSummaryRepository;
        }

        public async Task<Paging<PostMongoView>> GetPostViewPaged(string? cursor = null , int limit = 10 , bool ascending = false)
        {
            return await GetPostAsAggregate().ToPaged(cursor, limit, ascending);
        }

        public async Task<PostMongoView> GetByIdPostMongoView(string id)
        {
            return await GetPostAsAggregate()
                .Match(Builders<PostMongoView>.Filter.Eq(x => x.Id, id))
                .SingleOrDefaultAsync();
        }
      
        public IAggregateFluent<PostMongoView> GetPostAsAggregate()
        {
            return AsMongoCollection()
                .Aggregate()
                .Lookup<PostMongoEntity, UserMongoEntity, PostMongoView>(_userMongoRepository.AsMongoCollection(),
                    l => l.UserId,
                    f => f.Id,
                    r => r.User
                )
                .Unwind(x => x.User, new AggregateUnwindOptions<PostMongoView>() { PreserveNullAndEmptyArrays = true })
                .Lookup<PostMongoView, MediaMongoEntity, PostMongoView>(_mediaMongoRepository.AsMongoCollection(),
                    l => l.User.AvatarId,
                    f => f.Id,
                    r => r.User.Avatar
                )
               .Unwind(x => x.User.Avatar, new AggregateUnwindOptions<PostMongoView>() { PreserveNullAndEmptyArrays = true })
               .Lookup<PostMongoView, MediaMongoEntity, PostMongoView>(_mediaMongoRepository.AsMongoCollection(),
                   l => l.MediaId,
                   f => f.Id,
                   r => r.Media
               )
               .Unwind(x => x.Media, new AggregateUnwindOptions<PostMongoView> { PreserveNullAndEmptyArrays = true })
               .Lookup<PostMongoView, PostReactionSummaryMongoView, PostMongoView>(_postReactionSummaryRepository.AsMongoCollection(),
                   l => l.Id,
                   f => f.Id,
                   r => r.ReactionSummary
               )
               .Unwind(x => x.ReactionSummary, new AggregateUnwindOptions<PostMongoView> { PreserveNullAndEmptyArrays = true });

        }
    }
}

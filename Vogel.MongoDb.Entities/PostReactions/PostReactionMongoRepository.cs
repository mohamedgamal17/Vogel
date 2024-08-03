using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Common;
using Vogel.MongoDb.Entities.Extensions;
using Vogel.MongoDb.Entities.Medias;
using Vogel.MongoDb.Entities.Users;
namespace Vogel.MongoDb.Entities.PostReactions
{
    public class PostReactionMongoRepository : MongoRepository<PostReactionMongoEntity>
    {
        private readonly UserMongoRepository _userMongoRepository;
        private readonly IMongoRepository<MediaMongoEntity> _mediaRepository;
        public PostReactionMongoRepository(IMongoDatabase mongoDatabase, UserMongoRepository userMongoRepository, IMongoRepository<MediaMongoEntity> mediaRepository) : base(mongoDatabase)
        {
            _userMongoRepository = userMongoRepository;
            _mediaRepository = mediaRepository;
        }

        public async Task<Paging<PostReactionMongoView>> GetReactionViewPaged(string postId , string? cursor = null, int limit = 10 , bool ascending = false)
        {
            return await GetReactionAsAggregate()
                .Match(Builders<PostReactionMongoView>.Filter.Eq(x=> x.PostId, postId))
                .ToPaged(cursor, limit, ascending);
        }

        public async Task<PostReactionMongoView> GetReactionViewById(string postId,  string reactId)
        {
            return await GetReactionAsAggregate()
                .Match(Builders<PostReactionMongoView>.Filter.Eq(x=> x.PostId, postId) &
                    Builders<PostReactionMongoView>.Filter.Eq(x=> x.Id,  reactId))
                .SingleOrDefaultAsync();
        }

        public IAggregateFluent<PostReactionMongoView> GetReactionAsAggregate()
        {
            return AsMongoCollection()
                .Aggregate()
                .Lookup<PostReactionMongoEntity, UserMongoEntity, PostReactionMongoView>(_userMongoRepository.AsMongoCollection(),
                    l => l.UserId,
                    f => f.Id,
                    r => r.User
                )
                .Unwind(x => x.User, new AggregateUnwindOptions<PostReactionMongoView> { PreserveNullAndEmptyArrays = true })
                .Lookup<PostReactionMongoView, MediaMongoEntity, PostReactionMongoView>(_mediaRepository.AsMongoCollection(),
                    l => l.User.AvatarId,
                    f => f.Id,
                    r => r.User.Avatar
                )
                .Unwind(x => x.User.Avatar, new AggregateUnwindOptions<PostReactionMongoView> { PreserveNullAndEmptyArrays = true });
        }


    }
}

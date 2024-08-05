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

        public PostMongoRepository(IMongoDatabase mongoDatabase, UserMongoRepository userMongoRepository) : base(mongoDatabase)
        {
            _userMongoRepository = userMongoRepository;
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

        public async Task<Paging<PostMongoView>> GetUserFriendsPosts(string userId, string? cursor = null, int limit = 10, bool ascending = false)
        {
            var userFriends = await _userMongoRepository.GetUserRelationshipView(userId);

            if (userFriends == null)
            {
                return new Paging<PostMongoView>
                {
                    Data = new List<PostMongoView>(),
                    Info = new PagingInfo(null, null, ascending)
                };
            }

            return await GetPostAsAggregate()
                .Match(
                    Builders<PostMongoView>.Filter.In(x => x.UserId, userFriends.Friends.Select(x => x.UserId))
                  )
                .ToPaged(cursor, limit, ascending);
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
                .Lookup<PostMongoView, MediaMongoEntity, PostMongoView>(GetCollection<MediaMongoEntity>(MediaMongoConsts.CollectionName),
                    l => l.User.AvatarId,
                    f => f.Id,
                    r => r.User.Avatar
                )
               .Unwind(x => x.User.Avatar, new AggregateUnwindOptions<PostMongoView>() { PreserveNullAndEmptyArrays = true })
               .Lookup<PostMongoView, MediaMongoEntity, PostMongoView>(GetCollection<MediaMongoEntity>(MediaMongoConsts.CollectionName),
                   l => l.MediaId,
                   f => f.Id,
                   r => r.Media
               )
               .Unwind(x => x.Media, new AggregateUnwindOptions<PostMongoView> { PreserveNullAndEmptyArrays = true })
               .Lookup<PostMongoView, PostReactionSummaryMongoView, PostMongoView>(
                GetCollection<PostReactionSummaryMongoView>(PostReactionMongoConsts.ReactionSummaryView),
                   l => l.Id,
                   f => f.Id,
                   r => r.ReactionSummary
               )
               .Unwind(x => x.ReactionSummary, new AggregateUnwindOptions<PostMongoView> { PreserveNullAndEmptyArrays = true });

        }

    }
}

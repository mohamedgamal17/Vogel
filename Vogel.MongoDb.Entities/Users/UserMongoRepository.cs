
using MongoDB.Bson;
using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Common;
using Vogel.MongoDb.Entities.Extensions;
using Vogel.MongoDb.Entities.Friendship;
using Vogel.MongoDb.Entities.Medias;
namespace Vogel.MongoDb.Entities.Users
{
    public class UserMongoRepository : MongoRepository<UserMongoEntity>
    {
        private readonly MediaMongoRepository _mediaMongoRepository;
        private readonly FriendMongoRepository _frinedMongoRepository;
        public UserMongoRepository(IMongoDatabase mongoDatabase, MediaMongoRepository mediaMongoRepository, FriendMongoRepository frinedMongoRepository)
            : base(mongoDatabase)
        {
            _mediaMongoRepository = mediaMongoRepository;
            _frinedMongoRepository = frinedMongoRepository;
        }

        public async Task<Paging<UserMongoView>> GetUserViewPaged(string? cursor = null , bool ascending = false, int limit = 10)
        {
            var query = GetUserAsAggregate();

            return await query.ToPaged(cursor, limit, ascending);
        }

        public IAggregateFluent<UserMongoView> GetUserAsAggregate()
        {
            return AsMongoCollection()
                .Aggregate()
                .Lookup<UserMongoEntity, MediaMongoEntity, UserMongoView>(_mediaMongoRepository.AsMongoCollection(),
                    l => l.AvatarId,
                    f => f.Id,
                    r => r.Avatar
                )
                .Unwind(x => x.Avatar, new AggregateUnwindOptions<UserMongoView> { PreserveNullAndEmptyArrays = true });                         
        }

        public async Task<UserMongoView?> GetByIdUserMongoView(string id)
        {
            return await GetUserAsAggregate()
                 .Match(Builders<UserMongoView>.Filter.Eq(x => x.Id, id))
                 .SingleOrDefaultAsync();
        }

        public async Task<UserRelationshipView> GetUserRelationshipView(string userId)
        {
            return await AsMongoCollection()
                .Aggregate()
                .Match(x => x.Id == userId)
                .Lookup<UserMongoEntity, FriendMongoEntity, UserFriendView, List<UserFriendView>, UserRelationshipView>(
                _frinedMongoRepository.AsMongoCollection(),
                     new BsonDocument { { "userId", "$_id" } },
                     new EmptyPipelineDefinition<FriendMongoEntity>()
                        .Match(
                            Builders<FriendMongoEntity>.Filter.Eq(x => x.SourceId, "$$userId")
                                | Builders<FriendMongoEntity>.Filter.Eq(x => x.TargetId, "$$userId")
                         )
                        .Project(x => new UserFriendView
                        {
                            Id = x.Id,
                            UserId = x.SourceId == userId ? x.SourceId : x.TargetId
                        })
                    ,
                     x => x.Friends
                )
                .SingleOrDefaultAsync();
        }

    }
}

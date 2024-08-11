
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

        public UserMongoRepository(IMongoDatabase mongoDatabase)
            : base(mongoDatabase)
        {
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
                .Lookup<UserMongoEntity, MediaMongoEntity, UserMongoView>(GetCollection<MediaMongoEntity>(MediaMongoConsts.CollectionName),
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
                 MongoDatabase.GetCollection<FriendMongoEntity>(FriendshipMongoConsts.FriendCollection),
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
                .Project(x=> new UserRelationshipView
                {
                    Id = x.Id,
                    Friends =x.Friends
                })
                .SingleOrDefaultAsync();
        }

    }
}

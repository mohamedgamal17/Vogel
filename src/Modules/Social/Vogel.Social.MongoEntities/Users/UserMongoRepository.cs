using MongoDB.Bson;
using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Social.MongoEntities.Friendship;

namespace Vogel.Social.MongoEntities.Users
{
    public class UserMongoRepository : MongoRepository<UserMongoEntity>
    {

        public UserMongoRepository(IMongoDatabase mongoDatabase)
            : base(mongoDatabase)
        {
        }

        public async Task<Paging<UserMongoView>> GetUserViewPaged(string? cursor = null, bool ascending = false, int limit = 10)
        {
            var query = GetUserAsAggregate();

            return await query.ToPaged(cursor, limit, ascending);
        }

        public IAggregateFluent<UserMongoView> GetUserAsAggregate()
        {
            return AsMongoCollection()
                .Aggregate()
                .Project(x => new UserMongoView
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    BirthDate = x.BirthDate,
                    Gender = x.Gender,
                    AvatarId = x.AvatarId,
                    CreationTime = x.CreationTime,
                    CreatorId = x.CreatorId,
                    ModificationTime = x.ModificationTime,
                    ModifierId = x.ModifierId
                });
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
                 GetCollection<FriendMongoEntity>(FriendshipMongoConsts.FriendCollection),
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
                .Project(x => new UserRelationshipView
                {
                    Id = x.Id,
                    Friends = x.Friends
                })
                .SingleOrDefaultAsync();
        }

    }
}

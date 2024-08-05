using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Common;
using Vogel.MongoDb.Entities.Extensions;
using Vogel.MongoDb.Entities.Medias;
using Vogel.MongoDb.Entities.Users;
namespace Vogel.MongoDb.Entities.Friendship
{
    public class FriendMongoRepository : MongoRepository<FriendMongoEntity>
    {

        public FriendMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {

        }

        public async Task<Paging<FriendMongoView>> GetFriendViewPaged(string userId, string? cursor = null, int limit = 10, bool ascending = false)
        {
            return await GetFriendAsAggregate()
                .Match(
                Builders<FriendMongoView>.Filter.Eq(x=> x.TargetId, userId) | Builders<FriendMongoView>.Filter
                .Eq(x=> x.SourceId, userId))
                 .ToPaged(cursor, limit, ascending);
        }

        public async Task<FriendMongoView> GetFriendViewbyId(string id)
        {
            return await GetFriendAsAggregate()
                .Match(Builders<FriendMongoView>.Filter.Eq(x=> x.Id , id))
                .SingleOrDefaultAsync();
        }

        public IAggregateFluent<FriendMongoView> GetFriendAsAggregate()
        {
            return AsMongoCollection()
                .Aggregate()
                .Lookup<FriendMongoEntity, UserMongoEntity, FriendMongoView>(
                GetCollection<UserMongoEntity>(UserMongoConsts.CollectionName),
                    l => l.SourceId,
                    f => f.Id,
                    r => r.Source
                )
                .Unwind(x => x.Source, new AggregateUnwindOptions<FriendMongoView>() { PreserveNullAndEmptyArrays = true })
                .Lookup<FriendMongoView, MediaMongoEntity, FriendMongoView>(
                GetCollection<MediaMongoEntity>(MediaMongoConsts.CollectionName),
                    l => l.Source.AvatarId,
                    f => f.Id,
                    r => r.Source.Avatar
                )
               .Unwind(x => x.Source.Avatar, new AggregateUnwindOptions<FriendMongoView>() { PreserveNullAndEmptyArrays = true })
               .Lookup<FriendMongoView, UserMongoEntity, FriendMongoView>(GetCollection<UserMongoEntity>(UserMongoConsts.CollectionName),
                    l => l.TargetId,
                    f => f.Id,
                    r => r.Target
                )
                .Unwind(x => x.Target, new AggregateUnwindOptions<FriendMongoView>() { PreserveNullAndEmptyArrays = true })
                .Lookup<FriendMongoView, MediaMongoEntity, FriendMongoView>(
                GetCollection<MediaMongoEntity>(MediaMongoConsts.CollectionName),
                    l => l.Target.AvatarId,
                    f => f.Id,
                    r => r.Target.Avatar
                )
               .Unwind(x => x.Target.Avatar, new AggregateUnwindOptions<FriendMongoView>() { PreserveNullAndEmptyArrays = true });

        }

    }
}

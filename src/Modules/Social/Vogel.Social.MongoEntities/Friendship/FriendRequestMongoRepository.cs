using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Social.MongoEntities.Pictures;
using Vogel.Social.MongoEntities.Users;
namespace Vogel.Social.MongoEntities.Friendship
{
    public class FriendRequestMongoRepository : MongoRepository<FriendRequestMongoEntity>
    {

        public FriendRequestMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }

        public async Task<Paging<FriendRequestMongoView>> GetFriendRequestViewPaged(string userId, string? cursor = null, int limit = 10, bool ascending = false)
        {
            return await GetFriendRequestAsAggregate()
                 .Match(x => x.ReciverId == userId)
                .ToPaged(cursor, limit, ascending);
        }
        public async Task<FriendRequestMongoView> GetFriendRequestViewbyId(string id)
        {
            return await GetFriendRequestAsAggregate()
                .Match(x => x.Id == id)
                .SingleOrDefaultAsync();
        }

        public IAggregateFluent<FriendRequestMongoView> GetFriendRequestAsAggregate()
        {
            return AsMongoCollection()
                .Aggregate()
                .Lookup<FriendRequestMongoEntity, UserMongoEntity, FriendRequestMongoView>(
                   GetCollection<UserMongoEntity>(UserMongoConsts.CollectionName),
                    l => l.SenderId,
                    f => f.Id,
                    r => r.Sender
                )
                .Unwind(x => x.Sender, new AggregateUnwindOptions<FriendRequestMongoView>() { PreserveNullAndEmptyArrays = true })
                .Lookup<FriendRequestMongoView, PictureMongoEntity, FriendRequestMongoView>(GetCollection<PictureMongoEntity>(PictureMongoConsts.CollectionName),
                    l => l.Sender.AvatarId,
                    f => f.Id,
                    r => r.Sender.Avatar
                )
               .Unwind(x => x.Sender.Avatar, new AggregateUnwindOptions<FriendRequestMongoView>() { PreserveNullAndEmptyArrays = true })
               .Lookup<FriendRequestMongoView, UserMongoEntity, FriendRequestMongoView>(GetCollection<UserMongoEntity>(UserMongoConsts.CollectionName),
                    l => l.ReciverId,
                    f => f.Id,
                    r => r.Reciver
                )
                .Unwind(x => x.Reciver, new AggregateUnwindOptions<FriendRequestMongoView>() { PreserveNullAndEmptyArrays = true })
                .Lookup<FriendRequestMongoView, PictureMongoEntity, FriendRequestMongoView>(GetCollection<PictureMongoEntity>(PictureMongoConsts.CollectionName),
                    l => l.Reciver.AvatarId,
                    f => f.Id,
                    r => r.Reciver.Avatar
                )
               .Unwind(x => x.Reciver.Avatar, new AggregateUnwindOptions<FriendRequestMongoView>() { PreserveNullAndEmptyArrays = true });
        }
    }
}

using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Common;
using Vogel.MongoDb.Entities.Extensions;
using Vogel.MongoDb.Entities.Medias;
using Vogel.MongoDb.Entities.Users;
namespace Vogel.MongoDb.Entities.Friendship
{
    public class FriendRequestMongoRepository : MongoRepository<FriendRequestMongoEntity>
    {
        private readonly UserMongoRepository _userRepository;
        private readonly IMongoRepository<MediaMongoEntity> _mediaRepository;

        public FriendRequestMongoRepository(IMongoDatabase mongoDatabase, UserMongoRepository userRepository, IMongoRepository<MediaMongoEntity> mediaRepository) : base(mongoDatabase)
        {
            _userRepository = userRepository;
            _mediaRepository = mediaRepository;
        }

        public async Task<Paging<FriendRequestMongoView>> GetFriendRequestViewPaged(string userId , string? cursor = null , int limit = 10 , bool ascending = false)
        {
            return await GetFriendRequestAsAggregate()
                 .Match(x=> x.ReciverId == userId)
                .ToPaged(cursor, limit, ascending);
        }
        public async Task<FriendRequestMongoView> GetFriendRequestViewbyId(string id)
        {
            return await GetFriendRequestAsAggregate()
                .Match(x=> x.Id == id)
                .SingleOrDefaultAsync();
        }

        public IAggregateFluent<FriendRequestMongoView> GetFriendRequestAsAggregate()
        {       
            return AsMongoCollection()
                .Aggregate()
                .Lookup<FriendRequestMongoEntity, UserMongoEntity, FriendRequestMongoView>(_userRepository.AsMongoCollection(),
                    l => l.SenderId,
                    f => f.Id,
                    r => r.Sender
                )
                .Unwind(x => x.Sender, new AggregateUnwindOptions<FriendRequestMongoView>() { PreserveNullAndEmptyArrays = true })
                .Lookup<FriendRequestMongoView, MediaMongoEntity, FriendRequestMongoView>(_mediaRepository.AsMongoCollection(),
                    l => l.Sender.AvatarId,
                    f => f.Id,
                    r => r.Sender.Avatar
                )
               .Unwind(x => x.Sender.Avatar, new AggregateUnwindOptions<FriendRequestMongoView>() { PreserveNullAndEmptyArrays = true })
               .Lookup<FriendRequestMongoView, UserMongoEntity, FriendRequestMongoView>(_userRepository.AsMongoCollection(),
                    l => l.ReciverId,
                    f => f.Id,
                    r => r.Reciver
                )
                .Unwind(x => x.Reciver, new AggregateUnwindOptions<FriendRequestMongoView>() { PreserveNullAndEmptyArrays = true })
                .Lookup<FriendRequestMongoView, MediaMongoEntity, FriendRequestMongoView>(_mediaRepository.AsMongoCollection(),
                    l => l.Reciver.AvatarId,
                    f => f.Id,
                    r => r.Reciver.Avatar
                )
               .Unwind(x => x.Reciver.Avatar, new AggregateUnwindOptions<FriendRequestMongoView>() { PreserveNullAndEmptyArrays = true });
        }
    }
}

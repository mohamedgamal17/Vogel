using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Common;
using Vogel.MongoDb.Entities.Extensions;
using Vogel.MongoDb.Entities.Medias;
namespace Vogel.MongoDb.Entities.Users
{
    public class UserMongoRepository : MongoRepository<UserMongoEntity>
    {
        private readonly MediaMongoRepository _mediaMongoRepository;
        public UserMongoRepository(IMongoDatabase mongoDatabase, MediaMongoRepository mediaMongoRepository)
            : base(mongoDatabase)
        {
            _mediaMongoRepository = mediaMongoRepository;
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

    }
}

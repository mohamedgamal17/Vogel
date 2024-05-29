using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Medias;
namespace Vogel.MongoDb.Entities.Users
{
    public class UserMongoRepository : MongoRepository<UserMongoEntity, string>
    {
        protected override string CollectionName => "users";

        private readonly MediaMongoRepository _mediaMongoRepository;
        public UserMongoRepository(IMongoDatabase mongoDatabase, MediaMongoRepository mediaMongoRepository)
            : base(mongoDatabase)
        {
            _mediaMongoRepository = mediaMongoRepository;
        }

    }
}

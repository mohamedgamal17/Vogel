using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
namespace Vogel.MongoDb.Entities.Users
{
    public class UserMongoViewRepository : MongoRepository<UserMongoView, string>
    {
        protected override string CollectionName => "users_view";

        public UserMongoViewRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {

        }
        public IMongoCollection<PublicUserMongoView> AsPublicUserViewCollection()
        {
            return MongoDatabase.GetCollection<PublicUserMongoView>("users_view");
        }



    }
}

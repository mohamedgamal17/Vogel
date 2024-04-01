using Minio.DataModel.Notification;
using MongoDB.Driver;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Users.Dtos;
using Vogel.Domain;

namespace Vogel.Infrastructure.Presistance.Repositories
{
    public class UserRepository : MongoDbRepository<User>, IUserRepository
    {
        public UserRepository(MongoDbContext mongoDbContext) : base(mongoDbContext)
        {

        }

        public IAggregateFluent<PublicUserView> GetPublicUserView()
        {
            var mediaCollection = MongoDatabase.GetCollection<Media>(typeof(Media).Name);

            var aggregate = MongoDbCollection.Aggregate()
                .Lookup<User, Media, PublicUserView>(mediaCollection,
                    x => x.AvatarId,
                    x => x.Id,
                    x => x.Avatar
                )
                .Unwind<PublicUserView, PublicUserView>(x => x.Avatar, new AggregateUnwindOptions<PublicUserView> { PreserveNullAndEmptyArrays = true });

            return aggregate;
        }
    }
}

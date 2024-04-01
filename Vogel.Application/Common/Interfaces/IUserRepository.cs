using MongoDB.Driver;
using Vogel.Domain;

namespace Vogel.Application.Common.Interfaces
{
    public interface IUserRepository : IMongoDbRepository<User>
    {
        IAggregateFluent<PublicUserView> GetPublicUserView();
    }
}

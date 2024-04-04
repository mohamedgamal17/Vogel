using MongoDB.Driver;
using Vogel.Domain;

namespace Vogel.Application.Common.Interfaces
{
    public interface IPostRepository: IMongoDbRepository<Post>
    {
        IAggregateFluent<PostAggregateView> GetPostAggregateView();
    }
}

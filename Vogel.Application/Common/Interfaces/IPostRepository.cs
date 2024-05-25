using MongoDB.Driver;
using Vogel.Domain.Posts;

namespace Vogel.Application.Common.Interfaces
{
    public interface IPostRepository: IMongoDbRepository<Post>
    {
        IAggregateFluent<PostAggregateView> GetPostAggregateView();
    }
}

using MongoDB.Driver;
using Vogel.Domain;
using Vogel.Domain.Posts;

namespace Vogel.Application.Common.Interfaces
{
    public interface ICommentRepository : IMongoDbRepository<Comment>
    {
        IAggregateFluent<CommentAggregateView> GetCommentAggregateView();
    }
}

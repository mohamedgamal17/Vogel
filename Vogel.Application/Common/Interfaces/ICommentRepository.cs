using MongoDB.Driver;
using Vogel.Domain;

namespace Vogel.Application.Common.Interfaces
{
    public interface ICommentRepository : IMongoDbRepository<Comment>
    {
        IAggregateFluent<CommentAggregateView> GetCommentAggregateView();
    }
}

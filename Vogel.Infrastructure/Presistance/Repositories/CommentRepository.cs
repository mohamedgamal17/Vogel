using MongoDB.Driver;
using Vogel.Application.Common.Interfaces;
using Vogel.Domain;
using Vogel.Domain.Medias;
using Vogel.Domain.Posts;

namespace Vogel.Infrastructure.Presistance.Repositories
{
    public class CommentRepository : MongoDbRepository<Comment>, ICommentRepository
    {
        public CommentRepository(MongoDbContext mongoDbContext) : base(mongoDbContext)
        {
          
        }

        public IAggregateFluent<CommentAggregateView> GetCommentAggregateView()
        {
            var userCollection = MongoDatabase.GetCollection<User>(typeof(User).Name);
            var mediaCollection = MongoDatabase.GetCollection<Media>(typeof(Media).Name);

            var aggregate = MongoDbCollection.Aggregate()
                .Lookup<Comment, User, CommentAggregateView>(userCollection,
                    x => x.UserId,
                    x => x.Id,
                    x => x.User
                )
                .Unwind(x => x.User, new AggregateUnwindOptions<CommentAggregateView> { PreserveNullAndEmptyArrays = true })
                .Lookup<CommentAggregateView, Media, CommentAggregateView>(mediaCollection,
                    x => x.User.AvatarId,
                    x => x.Id,
                    x => x.User.Avatar
                )
                 .Unwind(x => x.User.Avatar, new AggregateUnwindOptions<CommentAggregateView> { PreserveNullAndEmptyArrays = true });

            return aggregate;
        }
    }
}

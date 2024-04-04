using MongoDB.Driver;
using Vogel.Application.Common.Interfaces;
using Vogel.Domain;

namespace Vogel.Infrastructure.Presistance.Repositories
{
    public class PostRepository : MongoDbRepository<Post>, IPostRepository
    {
        public PostRepository(MongoDbContext mongoDbContext) : base(mongoDbContext)
        {
        }

        public IAggregateFluent<PostAggregateView> GetPostAggregateView()
        {
            var userCollection = MongoDatabase.GetCollection<User>(typeof(User).Name);
            var mediaCollection = MongoDatabase.GetCollection<Media>(typeof(Media).Name);


            var aggregate = MongoDbCollection.Aggregate()
                .Lookup<Post, User, PostAggregateView>(userCollection,
                    x => x.UserId,
                    x => x.Id,
                    x => x.User
                )
                .Unwind(x => x.User, new AggregateUnwindOptions<PostAggregateView> { PreserveNullAndEmptyArrays = true })
                .Lookup<PostAggregateView, Media, PostAggregateView>(mediaCollection,
                    x => x.User.AvatarId,
                    x => x.Id,
                    x => x.User.Avatar
                )
                 .Unwind(x => x.User.Avatar, new AggregateUnwindOptions<PostAggregateView> { PreserveNullAndEmptyArrays = true });

            return aggregate;
        }
    }
}

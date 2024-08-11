using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Common;
using Vogel.MongoDb.Entities.Extensions;
using Vogel.MongoDb.Entities.Medias;
using Vogel.MongoDb.Entities.PostReactions;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.MongoDb.Entities.CommentReactions
{
    public class CommentReactionMongoRepository : MongoRepository<CommentReactionMongoEntity>
    {

        public CommentReactionMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {

        }

        public async Task<Paging<CommentReactionMongoView>> GetReactionViewPaged(string commentId , string? cursor = null, int limit = 10 , bool  ascending = false)
        {
            return await GetReactionAsAggregate().Match(x=> x.Id == commentId).ToPaged(cursor, limit, ascending);
        }

        public async Task<CommentReactionMongoView> GetReactionViewById(string commentId,string reactionId)
        {
            return await GetReactionAsAggregate()
                .Match(Builders<CommentReactionMongoView>.Filter.Eq(x=> x.CommentId , commentId) &
                    Builders<CommentReactionMongoView>.Filter.Eq(x=> x.Id, reactionId)
                ).SingleOrDefaultAsync();
        }

        public IAggregateFluent<CommentReactionMongoView> GetReactionAsAggregate()
        {
            return AsMongoCollection()
                .Aggregate()
                .Lookup<CommentReactionMongoEntity, UserMongoEntity, CommentReactionMongoView>(
                GetCollection<UserMongoEntity>(UserMongoConsts.CollectionName),
                    l => l.UserId,
                    f => f.Id,
                    r => r.User
                )
                .Unwind(x => x.User, new AggregateUnwindOptions<CommentReactionMongoView> { PreserveNullAndEmptyArrays = true })
                .Lookup<CommentReactionMongoView, MediaMongoEntity, CommentReactionMongoView>(GetCollection<MediaMongoEntity>(MediaMongoConsts.CollectionName),
                    l => l.User.AvatarId,
                    f => f.Id,
                    r => r.User.Avatar
                )
                .Unwind(x => x.User.Avatar, new AggregateUnwindOptions<CommentReactionMongoView> { PreserveNullAndEmptyArrays = true });

        }

        public IMongoQueryable<CommentReactionSummaryMongoView> GetReactionSummaryViewAsQuerable()
        {
            var query = from reaction in AsQuerable()
                        group reaction by reaction.CommentId into grouped
                        select new CommentReactionSummaryMongoView
                        {
                            Id = grouped.Key,
                            TotalLike = grouped.Where(x => x.Type == ReactionType.Like).Count(),
                            TotalLove = grouped.Where(x => x.Type == ReactionType.Love).Count(),
                            TotalAngry = grouped.Where(x => x.Type == ReactionType.Angry).Count(),
                            TotalLaugh = grouped.Where(x => x.Type == ReactionType.Laugh).Count(),
                            TotalSad = grouped.Where(x => x.Type == ReactionType.Sad).Count()
                        };

            return query;
        }
    }
}

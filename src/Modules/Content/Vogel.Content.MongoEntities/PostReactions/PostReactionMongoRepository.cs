using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.BuildingBlocks.Shared.Models;

namespace Vogel.Content.MongoEntities.PostReactions
{
    public class PostReactionMongoRepository : MongoRepository<PostReactionMongoEntity>
    {
        public PostReactionMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {

        }

        public async Task<Paging<PostReactionSummaryMongoView>> ListPostsReactionsSummary(IEnumerable<string> ids , string? cursor = null , int limit = 10, bool ascending = false)
        {
            var query = AsMongoCollection()
                .Aggregate()
                .Match(Filter.In(x => x.PostId, ids))
                .Group(k => k.PostId, grouped => new PostReactionSummaryMongoView
                {
                    Id = grouped.Key,
                    TotalLike = grouped.Where(x => x.Type == ReactionType.Like).Count(),
                    TotalLove = grouped.Where(x => x.Type == ReactionType.Love).Count(),
                    TotalAngry = grouped.Where(x => x.Type == ReactionType.Angry).Count(),
                    TotalLaugh = grouped.Where(x => x.Type == ReactionType.Laugh).Count(),
                    TotalSad = grouped.Where(x => x.Type == ReactionType.Sad).Count()
                });

            var data = await query.ToPaged(cursor, limit, ascending);

            return data;
        }

        public async Task<PostReactionSummaryMongoView?> GetPostReactionSummary(string postId)
        {
            var query = AsMongoCollection()
             .Aggregate()
             .Match(Filter.Eq(x => x.PostId, postId))
             .Group(k => k.PostId, grouped => new PostReactionSummaryMongoView
             {
                 Id = grouped.Key,
                 TotalLike = grouped.Where(x => x.Type == ReactionType.Like).Count(),
                 TotalLove = grouped.Where(x => x.Type == ReactionType.Love).Count(),
                 TotalAngry = grouped.Where(x => x.Type == ReactionType.Angry).Count(),
                 TotalLaugh = grouped.Where(x => x.Type == ReactionType.Laugh).Count(),
                 TotalSad = grouped.Where(x => x.Type == ReactionType.Sad).Count()
             });

            return await query.SingleOrDefaultAsync();
        }

    }
}

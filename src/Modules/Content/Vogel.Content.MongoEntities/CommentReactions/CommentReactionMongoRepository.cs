using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.MongoDb.Extensions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Content.MongoEntities.PostReactions;
namespace Vogel.Content.MongoEntities.CommentReactions
{
    public class CommentReactionMongoRepository : MongoRepository<CommentReactionMongoEntity>
    {
        public CommentReactionMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {

        }

        public async Task<Paging<CommentReactionSummaryMongoView>> ListCommentsReactionsSummary(IEnumerable<string> ids, string? cursor = null, int limit = 10, bool ascending = false)
        {
            var query = AsMongoCollection()
                .Aggregate()
                .Match(Filter.In(x => x.CommentId, ids))
                .Group(k => k.CommentId, grouped => new CommentReactionSummaryMongoView
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

        public async Task<CommentReactionSummaryMongoView?> GetCommentReactionSummary(string postId)
        {
            var query = AsMongoCollection()
             .Aggregate()
             .Match(Filter.Eq(x => x.CommentId, postId))
             .Group(k => k.CommentId, grouped => new CommentReactionSummaryMongoView
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

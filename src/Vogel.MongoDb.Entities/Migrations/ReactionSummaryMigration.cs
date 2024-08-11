using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb.Migrations;
using Vogel.MongoDb.Entities.CommentReactions;
using Vogel.MongoDb.Entities.PostReactions;

namespace Vogel.MongoDb.Entities.Migrations
{
    public class ReactionSummaryMigration : IMongoDbMigration
    {
        public int Version => 1722709982;

        public async Task Up(IMongoDatabase mongoDb)
        {
            var commentReactionPipline = new EmptyPipelineDefinition<CommentReactionMongoEntity>()
                .Group(x => x.CommentId, grouped => new CommentReactionSummaryMongoView
                {
                    Id = grouped.Key,
                    TotalLike = grouped.Where(x => x.Type == ReactionType.Like).Count(),
                    TotalLove = grouped.Where(x => x.Type == ReactionType.Love).Count(),
                    TotalAngry = grouped.Where(x => x.Type == ReactionType.Angry).Count(),
                    TotalSad = grouped.Where(x => x.Type == ReactionType.Sad).Count(),
                    TotalLaugh = grouped.Where(x => x.Type == ReactionType.Laugh).Count(),
                });


            var postReactionPipline = new EmptyPipelineDefinition<PostReactionMongoEntity>()
                .Group(x => x.PostId, grouped => new PostReactionSummaryMongoView
                {
                    Id = grouped.Key,
                    TotalLike = grouped.Where(x => x.Type == ReactionType.Like).Count(),
                    TotalLove = grouped.Where(x => x.Type == ReactionType.Love).Count(),
                    TotalAngry = grouped.Where(x => x.Type == ReactionType.Angry).Count(),
                    TotalSad = grouped.Where(x => x.Type == ReactionType.Sad).Count(),
                    TotalLaugh = grouped.Where(x => x.Type == ReactionType.Laugh).Count(),
                });


            await mongoDb.CreateViewAsync("comment_reactions_summary_view", "comment_reactions", commentReactionPipline);
            await mongoDb.CreateViewAsync("post_reactions_summary_view", "post_reactions", commentReactionPipline);
        }

    }
}

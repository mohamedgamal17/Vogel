using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb.Migrations;
using Vogel.MongoDb.Entities.CommentReactions;
using Vogel.MongoDb.Entities.Comments;
using Vogel.MongoDb.Entities.Medias;
using Vogel.MongoDb.Entities.PostReactions;
using Vogel.MongoDb.Entities.Posts;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.MongoDb.Entities.Migrations
{
    public class IncludeSummaryViewMigration : IMongoDbMigration
    {
        public int Version => 1721849159;

        public async Task Up(IMongoDatabase mongoDb)
        {
            await mongoDb.DropCollectionAsync("posts_view");


            var postReactionSummaryPipline = new EmptyPipelineDefinition<PostReactionMongoEntity>()
                .Group(x => x.PostId, gr => new PostReactionSummaryMonogView
                {
                    Id = gr.Key,
                    TotalLike = gr.Sum(x => x.Type == ReactionType.Like ? 1 : 0),
                    TotalLove = gr.Sum(x => x.Type == ReactionType.Love ? 1 : 0),
                    TotalLaugh = gr.Sum(x => x.Type == ReactionType.Laugh ? 1 : 0),
                    TotalAngry = gr.Sum(x => x.Type == ReactionType.Angry ? 1 : 0),
                    TotalSad = gr.Sum(x => x.Type == ReactionType.Sad ? 1 : 0)
                });

            await mongoDb.CreateViewAsync("post_reactions_summary_view", "post_reactions_view", postReactionSummaryPipline);


            var postViewPipline = new EmptyPipelineDefinition<PostMongoEntity>()
                .Lookup<PostMongoEntity, PostMongoEntity, UserMongoView, PostMongoView>(mongoDb.GetCollection<UserMongoView>("users_view"), "userId", "_id", "user")
                .Unwind("user", new AggregateUnwindOptions<PostMongoView> { PreserveNullAndEmptyArrays = true })
                .Lookup<PostMongoEntity, PostMongoView, MediaMongoEntity, PostMongoView>(mongoDb.GetCollection<MediaMongoEntity>("medias"),
                    "mediaId", "_id", "media")
                .Unwind("media", new AggregateUnwindOptions<PostMongoView> { PreserveNullAndEmptyArrays = true })
                .Lookup<PostMongoEntity, PostMongoView, PostReactionSummaryMonogView, PostMongoView>(mongoDb.GetCollection<PostReactionSummaryMonogView>("post_reactions_summary_view"), x => x.Id, x => x.Id, c => c.ReactionSummary)
                .Unwind(x => x.ReactionSummary, new AggregateUnwindOptions<PostReactionMongoView> { PreserveNullAndEmptyArrays = true });


            await mongoDb.CreateViewAsync("posts_view", "posts", postViewPipline);


            await mongoDb.DropCollectionAsync("comments_view");

            var commentReactionPipline = new EmptyPipelineDefinition<CommentReactionMongoEntity>()
                .Group(x => x.CommentId, gr => new PostReactionSummaryMonogView
                {
                    Id = gr.Key,
                    TotalLike = gr.Sum(x => x.Type == ReactionType.Like ? 1 : 0),
                    TotalLove = gr.Sum(x => x.Type == ReactionType.Love ? 1 : 0),
                    TotalLaugh = gr.Sum(x => x.Type == ReactionType.Laugh ? 1 : 0),
                    TotalAngry = gr.Sum(x => x.Type == ReactionType.Angry ? 1 : 0),
                    TotalSad = gr.Sum(x => x.Type == ReactionType.Sad ? 1 : 0)
                });


            await mongoDb.CreateViewAsync("comment_reactions_summary_view", "comment_reactions", commentReactionPipline);


            var commentViewPipline = new EmptyPipelineDefinition<CommentMongoEntity>()
            .Lookup<CommentMongoEntity, CommentMongoEntity, PublicUserMongoView, CommentMongoView>(mongoDb.GetCollection<PublicUserMongoView>("users_view"), "userId", "_id", "user")
            .Unwind("user", new AggregateUnwindOptions<CommentMongoView> { PreserveNullAndEmptyArrays = true })
            .Lookup<CommentMongoEntity, CommentMongoView, CommentReactionSummaryMongoView, CommentMongoView>(mongoDb.GetCollection<CommentReactionSummaryMongoView>("comment_reactions_summary_view"), "_id", "_id", "reactionSummary")
            .Unwind("reactionSummary", new AggregateUnwindOptions<CommentMongoView> { PreserveNullAndEmptyArrays = true });

            await mongoDb.CreateViewAsync("comments_view", "comments", commentViewPipline);
        }
    }
}

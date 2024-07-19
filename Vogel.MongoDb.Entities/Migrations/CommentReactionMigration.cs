using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb.Migrations;
using Vogel.MongoDb.Entities.CommentReactions;
using Vogel.MongoDb.Entities.Comments;
using Vogel.MongoDb.Entities.PostReactions;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.MongoDb.Entities.Migrations
{
    internal class CommentReactionMigration : IMongoDbMigration
    {
        public int Version => 1721412046;

        public async Task Up(IMongoDatabase mongoDb)
        {
            await mongoDb.CreateCollectionAsync("comment_reactions");

            var pipline = new EmptyPipelineDefinition<CommentReactionMongoEntity>()
                .Lookup<CommentReactionMongoEntity, CommentReactionMongoEntity, PublicUserMongoView, CommentReactionMongoView>(mongoDb.GetCollection<PublicUserMongoView>("users_view"), "userId", "_id", "user")
                .Unwind("user", new AggregateUnwindOptions<CommentMongoView> { PreserveNullAndEmptyArrays = true });

            await mongoDb.CreateViewAsync("comment_reactions_view", "comment_reactions", pipline);
        }
    }
}

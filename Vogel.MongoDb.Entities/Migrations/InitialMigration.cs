using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Operations;
using Vogel.BuildingBlocks.MongoDb.Migrations;
using Vogel.MongoDb.Entities.Comments;
using Vogel.MongoDb.Entities.Medias;
using Vogel.MongoDb.Entities.PostReactions;
using Vogel.MongoDb.Entities.Posts;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.MongoDb.Entities.Migrations
{
    public class InitialMigration : IMongoDbMigration
    {
        public int Version => 1716936383;

        public async Task Up(IMongoDatabase mongoDb)
        {
            await mongoDb.CreateCollectionAsync("posts");

            await mongoDb.CreateCollectionAsync("users");

            await mongoDb.CreateCollectionAsync("medias");

            await mongoDb.CreateCollectionAsync("post_reactions");

            await mongoDb.CreateCollectionAsync("comment_reactions");

            await mongoDb.CreateCollectionAsync("comments");

            await mongoDb.CreateCollectionAsync("friends");

            await mongoDb.CreateCollectionAsync("friend_requests");
        }
    }
}

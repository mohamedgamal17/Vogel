﻿using MongoDB.Bson;
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
    internal class InitialMigration : IMongoDbMigration
    {
        public int Version => 1716936383;

        public async Task Up(IMongoDatabase mongoDb)
        {
            await mongoDb.CreateCollectionAsync("posts");

            await mongoDb.CreateCollectionAsync("users");

            await mongoDb.CreateCollectionAsync("medias");

            await mongoDb.CreateCollectionAsync("post_reactions");

            await mongoDb.CreateCollectionAsync("comments");


            var userViewPipline = new EmptyPipelineDefinition<UserMongoEntity>()
                .Lookup<UserMongoEntity, UserMongoEntity, MediaMongoEntity, UserMongoView>(mongoDb.GetCollection<MediaMongoEntity>("medias"), "avatarId", "_id", "avatar")
                .Unwind("avatar", new AggregateUnwindOptions<UserMongoView> { PreserveNullAndEmptyArrays = true });


            await mongoDb.CreateViewAsync("users_view", "users", userViewPipline);


            var postViewPipline = new EmptyPipelineDefinition<PostMongoEntity>()
                .Lookup<PostMongoEntity, PostMongoEntity, UserMongoView, PostMongoView>(mongoDb.GetCollection<UserMongoView>("users_view"), "userId", "_id", "user")
                .Unwind("user", new AggregateUnwindOptions<PostMongoView> { PreserveNullAndEmptyArrays = true })
                .Lookup<PostMongoEntity, PostMongoView, MediaMongoEntity, PostMongoView>(mongoDb.GetCollection<MediaMongoEntity>("medias"),
                    "mediaId", "_id", "media")
                .Unwind("media", new AggregateUnwindOptions<PostMongoView> { PreserveNullAndEmptyArrays = true });

            await mongoDb.CreateViewAsync("posts_view", "posts", postViewPipline);


            var commentViewPipline = new EmptyPipelineDefinition<CommentMongoEntity>()
                .Lookup<CommentMongoEntity, CommentMongoEntity, PublicUserMongoView, CommentMongoView>(mongoDb.GetCollection<PublicUserMongoView>("users_view"), "userId", "_id", "user")
                .Unwind("user", new AggregateUnwindOptions<CommentMongoView> { PreserveNullAndEmptyArrays = true });


            await mongoDb.CreateViewAsync("comments_view", "comments", commentViewPipline);

            var reactionViewPipline = new EmptyPipelineDefinition<PostReactionMongoEntity>()
                .Lookup<PostReactionMongoEntity, PostReactionMongoEntity, PublicUserMongoView, PostReactionMongoView>(mongoDb.GetCollection<PublicUserMongoView>("users_view"),"userId" , "_id", "user")
                .Unwind("user", new AggregateUnwindOptions<CommentMongoView> { PreserveNullAndEmptyArrays = true });

            await mongoDb.CreateViewAsync("post_reactions_view", "post_reactions", reactionViewPipline);

        }
    }
}
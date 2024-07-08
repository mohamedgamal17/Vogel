using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb.Migrations;
using Vogel.MongoDb.Entities.Friendship;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.MongoDb.Entities.Migrations
{
    internal class FriendshipMigration : IMongoDbMigration
    {
        public int Version => 1719700535;

        public async Task Up(IMongoDatabase mongoDb)
        {
            await mongoDb.CreateCollectionAsync("friends");

            await mongoDb.CreateCollectionAsync("friend_requests");

            var friendViewPipline = new EmptyPipelineDefinition<FriendMongoEntity>()
                 .Lookup<FriendMongoEntity, FriendMongoEntity, UserMongoView, FriendMongoView>(mongoDb.GetCollection<UserMongoView>("users_view"), "sourceId", "_id", "source")
                 .Unwind("source", new AggregateUnwindOptions<FriendMongoView> { PreserveNullAndEmptyArrays = true })
                 .Lookup<FriendMongoEntity, FriendMongoView, UserMongoView, FriendMongoView>(mongoDb.GetCollection<UserMongoView>("users_view"), "targetId", "_id", "target")
                 .Unwind("target", new AggregateUnwindOptions<FriendMongoView> { PreserveNullAndEmptyArrays = true });

            await mongoDb.CreateViewAsync("friends_view", "friends", friendViewPipline);


            var friendRequestViewPipline = new EmptyPipelineDefinition<FriendRequestMongoEntity>()
                 .Lookup<FriendRequestMongoEntity, FriendRequestMongoEntity, UserMongoView, FriendRequestMongoView>(mongoDb.GetCollection<UserMongoView>("users_view"), "senderId", "_id", "sender")
                 .Unwind("sender", new AggregateUnwindOptions<FriendRequestMongoView> { PreserveNullAndEmptyArrays = true })
                 .Lookup<FriendRequestMongoEntity, FriendRequestMongoView, UserMongoView, FriendRequestMongoView>(mongoDb.GetCollection<UserMongoView>("users_view"), "reciverId", "_id", "reciver")
                 .Unwind("reciver", new AggregateUnwindOptions<FriendRequestMongoView> { PreserveNullAndEmptyArrays = true });

            await  mongoDb.CreateViewAsync("friend_requests_view", "friend_requests", friendRequestViewPipline);

        }
    }
}

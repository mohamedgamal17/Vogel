﻿using Vogel.BuildingBlocks.MongoDb;
namespace Vogel.MongoDb.Entities.Friendship
{
    [MongoCollection(FriendshipMongoConsts.FriendRequestCollection)]
    public class FriendRequestMongoEntity : FullAuditedMongoEntity<string>
    {
        public string SenderId { get; set; }
        public string ReciverId { get; set; }
        public FriendRequestState State { get; set; }
    }
}
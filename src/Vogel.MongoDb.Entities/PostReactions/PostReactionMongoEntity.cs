﻿using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.PostReactions
{
    [MongoCollection(PostReactionMongoConsts.CollectioName)]
    public class PostReactionMongoEntity : FullAuditedMongoEntity<string>
    {
        public string UserId { get; set; }
        public string PostId { get; set; }
        public ReactionType Type { get; set; }
    }
}
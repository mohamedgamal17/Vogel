﻿using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.Reactions
{
    public class ReactionMongoEntity : FullAuditedMongoEntity<string>
    {
        public string UserId { get; set; }
        public string PostId { get; set; }
        public ReactionType Type { get; set; }
    }
}

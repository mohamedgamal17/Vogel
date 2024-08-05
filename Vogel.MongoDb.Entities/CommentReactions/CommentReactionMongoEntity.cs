﻿using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.PostReactions;

namespace Vogel.MongoDb.Entities.CommentReactions
{

    [MongoCollection(CommentReactionMongoConsts.CollectionName)]
    public class CommentReactionMongoEntity : FullAuditedMongoEntity<string>
    {
        public string UserId { get; set; }

        public string CommentId { get; set; }

        public ReactionType Type { get; set; }
    }
}

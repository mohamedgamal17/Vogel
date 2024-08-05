﻿
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.CommentReactions
{
    [MongoCollection(CommentReactionMongoConsts.ReactionSummarView)]
    public class CommentReactionSummaryMongoView : MongoEntity
    {
        public long TotalLike { get; set; }
        public long TotalLove { get; set; }
        public long TotalLaugh { get; set; }
        public long TotalSad { get; set; }
        public long TotalAngry { get; set; }
    }
}

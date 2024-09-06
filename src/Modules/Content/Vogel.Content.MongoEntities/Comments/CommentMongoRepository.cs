using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.MongoEntities.CommentReactions;
using Vogel.Content.MongoEntities.PostReactions;
using MongoDB.Driver.Linq;
namespace Vogel.Content.MongoEntities.Comments
{
    public class CommentMongoRepository : MongoRepository<CommentMongoEntity>
    {
        private readonly IMongoRepository<CommentReactionMongoEntity> _commentReactionRepository;
        public CommentMongoRepository(IMongoDatabase mongoDatabase, IMongoRepository<CommentReactionMongoEntity> commentReactionRepository) : base(mongoDatabase)
        {
            _commentReactionRepository = commentReactionRepository;
        }

        public async Task<CommentMongoView> GetCommentViewById(string postId,string id)
        {
            var reactionQuery = from react in _commentReactionRepository.AsQuerable()
                                group react by react.CommentId into grouped
                                select new CommentReactionSummaryMongoView
                                {
                                    Id = grouped.Key,
                                    TotalLike = grouped.Where(x => x.Type == ReactionType.Like).Count(),
                                    TotalLove = grouped.Where(x => x.Type == ReactionType.Love).Count(),
                                    TotalAngry = grouped.Where(x => x.Type == ReactionType.Angry).Count(),
                                    TotalLaugh = grouped.Where(x => x.Type == ReactionType.Laugh).Count(),
                                    TotalSad = grouped.Where(x => x.Type == ReactionType.Sad).Count()
                                };

            var query = from comment in AsQuerable()
                        join reactSummary in reactionQuery
                        on comment.Id equals reactSummary.Id
                        select new CommentMongoView
                        {
                            Id = comment.Id,
                            Content = comment.Content,
                            UserId = comment.UserId,
                            ReactionSummary = reactSummary,
                            CommentId = comment.CommentId,
                            PostId = comment.PostId,
                            CreationTime = comment.CreationTime,
                            CreatorId = comment.CreatorId,
                            ModifierId = comment.ModifierId,
                            ModificationTime = comment.ModificationTime,
                            DeletorId = comment.DeletorId,
                            DeletionTime = comment.DeletionTime
                        };


            return await query.SingleOrDefaultAsync(x =>x.PostId == postId && x.Id == id);
        }

    }
}

using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.MongoEntities.CommentReactions;
using MongoDB.Driver.Linq;
namespace Vogel.Content.MongoEntities.Comments
{
    public class CommentMongoRepository : MongoRepository<CommentMongoEntity>
    {
        private readonly CommentReactionMongoRepository _commentReactionRepository;
        public CommentMongoRepository(IMongoDatabase mongoDatabase, CommentReactionMongoRepository commentReactionRepository) : base(mongoDatabase)
        {
            _commentReactionRepository = commentReactionRepository;
        }

        public async Task<CommentMongoView?> GetCommentViewById(string postId,string commentId)
        {
            var query = AsMongoCollection()
                .Aggregate()
                .Match(
                    Filter.And(
                        Filter.Eq(x=> x.PostId , postId),
                        Filter.Eq(x=> x.CommentId,  commentId)
                    )
                );

            var result = await ProjectCommentViewQuery(query).SingleOrDefaultAsync();

            if(result != null)
            {
                var reactionSummary = await _commentReactionRepository.GetCommentReactionSummary(commentId);

                result.ReactionSummary = reactionSummary;
            }

            return result;
        }



        private IAggregateFluent<CommentMongoView> ProjectCommentViewQuery(IAggregateFluent<CommentMongoEntity> query)
        {
            return query.Project(comment => new CommentMongoView
            {
                Id = comment.Id,
                Content = comment.Content,
                UserId = comment.UserId,
                CommentId = comment.CommentId,
                PostId = comment.PostId,
                CreationTime = comment.CreationTime,
                CreatorId = comment.CreatorId,
                ModifierId = comment.ModifierId,
                ModificationTime = comment.ModificationTime,
                DeletorId = comment.DeletorId,
                DeletionTime = comment.DeletionTime
            });
        }

    }
}

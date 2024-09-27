using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.MongoEntities.CommentReactions;
using MongoDB.Driver.Linq;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.MongoDb.Extensions;
namespace Vogel.Content.MongoEntities.Comments
{
    public class CommentMongoRepository : MongoRepository<CommentMongoEntity>
    {
        private readonly CommentReactionMongoRepository _commentReactionRepository;
        public CommentMongoRepository(IMongoDatabase mongoDatabase, CommentReactionMongoRepository commentReactionRepository) : base(mongoDatabase)
        {
            _commentReactionRepository = commentReactionRepository;
        }

        public async Task<Paging<CommentMongoView>> ListCommentView(string postId,  string? cursor = null,int limit = 10 , bool ascending = false)
        {
            var query = AsMongoCollection()
                .Aggregate()
                .Match(
                    Filter.Eq(x => x.PostId, postId)
                );

            var paged = await ProjectCommentViewQuery(query).ToPaged(cursor, limit, ascending);

            if(paged.Data.Count > 0)
            {
                var ids = paged.Data.Select(x => x.Id).ToList();

                var reactionSummaries = await _commentReactionRepository.ListCommentsReactionsSummary(ids, limit: paged.Data.Count);

                var mappedReactions = reactionSummaries.Data.ToDictionary((x) => x.Id, x => x);

                paged.Data.ForEach(d =>
                {
                    d.ReactionSummary = mappedReactions.GetValueOrDefault(d.Id);
                });
            }

            return paged;
        }

        public async Task<CommentMongoView?> GetCommentViewById(string postId,string commentId)
        {
            var query = AsMongoCollection()
                .Aggregate()
                .Match(
                    Filter.And(
                        Filter.Eq(x=> x.PostId , postId),
                        Filter.Eq(x=> x.Id,  commentId)
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

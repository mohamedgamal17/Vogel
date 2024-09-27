using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using MongoDB.Driver.Linq;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.MongoDb.Extensions;
namespace Vogel.Content.MongoEntities.Comments
{
    public class CommentMongoRepository : MongoRepository<CommentMongoEntity>
    {
        public CommentMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }

        public async Task<Paging<CommentMongoView>> ListCommentView(string postId,  string? cursor = null,int limit = 10 , bool ascending = false)
        {
            var query = AsMongoCollection()
                .Aggregate()
                .Match(
                    Filter.Eq(x => x.PostId, postId)
                );

            var paged = await ProjectCommentViewQuery(query).ToPaged(cursor, limit, ascending);

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

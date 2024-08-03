using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.CommentReactions;
using Vogel.MongoDb.Entities.Common;
using Vogel.MongoDb.Entities.Extensions;
using Vogel.MongoDb.Entities.Medias;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.MongoDb.Entities.Comments
{
    public class CommentMongoRepository : MongoRepository<CommentMongoEntity>
    {
        private readonly UserMongoRepository _userMongoRepository;
        private readonly CommentReactionMongoRepository _commentReactionMongoRepository;
        private readonly IMongoRepository<MediaMongoEntity> _mediaRepository;
        private readonly IMongoRepository<CommentReactionSummaryMongoView> _commentReactionSummaryRepository;
        public CommentMongoRepository(IMongoDatabase mongoDatabase, UserMongoRepository userMongoRepository, CommentReactionMongoRepository commentReactionMongoRepository, IMongoRepository<MediaMongoEntity> mediaRepository, IMongoRepository<CommentReactionSummaryMongoView> commentReactionSummaryRepository) : base(mongoDatabase)
        {
            _userMongoRepository = userMongoRepository;
            _commentReactionMongoRepository = commentReactionMongoRepository;
            _mediaRepository = mediaRepository;
            _commentReactionSummaryRepository = commentReactionSummaryRepository;
        }


        public async Task<Paging<CommentMongoView>> GetCommentViewPaged(string postId , string? commentId = null , string? cursor = null , int limit = 10 , bool ascending = false)
        {
            var query = GetCommentAsAggregate();

            if(commentId != null)
            {
                query = query.Match(Builders<CommentMongoView>.Filter.Eq(x => x.CommentId, commentId));
            }

            return await query.ToPaged(cursor, limit, ascending);
        }

        public async Task<CommentMongoView> GetCommentViewById(string commentId)
        {
            return await GetCommentAsAggregate()
                .Match(Builders<CommentMongoView>.Filter.Eq(x=> x.Id ,commentId))
                .SingleOrDefaultAsync();
        }

        public IAggregateFluent<CommentMongoView> GetCommentAsAggregate()
        {
            return AsMongoCollection()
                .Aggregate()
                .Lookup<CommentMongoEntity, UserMongoEntity, CommentMongoView>(_userMongoRepository.AsMongoCollection(),
                    l=> l.UserId,
                    f=> f.Id,
                    r=> r.User
                )
                .Unwind(x=> x.User , new AggregateUnwindOptions<CommentMongoView> { PreserveNullAndEmptyArrays =true})
                .Lookup<CommentMongoView, MediaMongoEntity,CommentMongoView>(_mediaRepository.AsMongoCollection(),
                    l=> l.User.AvatarId,
                    f=> f.Id,
                    r=> r.User.Avatar
                )
                .Unwind(x=>x.User.Avatar, new AggregateUnwindOptions<CommentMongoView> {PreserveNullAndEmptyArrays =true })
                .Lookup<CommentMongoView, CommentReactionSummaryMongoView, CommentMongoView>(_commentReactionSummaryRepository.AsMongoCollection(),
                 l=> l.Id,
                 f=>f.Id,
                 r=> r.ReactionSummary
               )
                .Unwind(x=> x.ReactionSummary, new AggregateUnwindOptions<CommentMongoView> { PreserveNullAndEmptyArrays = true });
        }
    }



}

using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Content.MongoEntities.Medias;
using Vogel.Content.MongoEntities.PostReactions;
using MongoDB.Driver.Linq;
namespace Vogel.Content.MongoEntities.Posts
{
    public class PostMongoRepository : MongoRepository<PostMongoEntity>
    {
        private readonly IMongoRepository<PostReactionMongoEntity> _postReactionRepository;
        private readonly IMongoRepository<MediaMongoEntity> _mediaRepository;
        public PostMongoRepository(IMongoDatabase mongoDatabase, IMongoRepository<PostReactionMongoEntity> postReactionRepository, IMongoRepository<MediaMongoEntity> mediaRepository) : base(mongoDatabase)
        {
            _postReactionRepository = postReactionRepository;
            _mediaRepository = mediaRepository;
        }

        public async Task<PostMongoView> GetPostViewById(string id)
        {
            var reactionQuery = from react in _postReactionRepository.AsQuerable()
                                group react by react.PostId into grouped
                                select new PostReactionSummaryMongoView
                                {
                                    Id = grouped.Key,
                                    TotalLike = grouped.Where(x => x.Type == ReactionType.Like).Count(),
                                    TotalLove = grouped.Where(x => x.Type == ReactionType.Love).Count(),
                                    TotalAngry = grouped.Where(x => x.Type == ReactionType.Angry).Count(),
                                    TotalLaugh = grouped.Where(x => x.Type == ReactionType.Laugh).Count(),
                                    TotalSad = grouped.Where(x => x.Type == ReactionType.Sad).Count()
                                };

            var query = from post in AsQuerable()
                        join media in _mediaRepository.AsQuerable()
                        on post.MediaId equals media.Id
                        join reactSummary in reactionQuery
                        on post.Id equals reactSummary.Id
                        select new PostMongoView
                        {
                            Id = post.Id,
                            Caption = post.Caption,
                            UserId = post.UserId,
                            MediaId = post.MediaId,
                            Media = media,
                            ReactionSummary = reactSummary,
                            CreatorId = post.CreatorId,
                            CreationTime = post.CreationTime,
                            ModifierId = post.ModifierId,
                            ModificationTime = post.ModificationTime,
                            DeletorId = post.DeletorId,
                            DeletionTime = post.DeletionTime
                        };

            return await query.SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}

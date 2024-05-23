using MongoDB.Driver;
using Vogel.BuildingBlocks.MongoDb;

namespace Vogel.MongoDb.Entities.Posts
{
    public class PostReactionMongoEntity : FullAuditedMongoEntity<string>
    {
        public string UserId { get; set; }
        public string PostId { get; set; }
        public ReactionType Type { get; set; }
    }

    public class PostReactionMongoRepository : MongoRepository<PostMongoEntity, string>
    {
        protected override string CollectionName => "post_reactions";
        public PostReactionMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }
    }
}

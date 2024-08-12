using Vogel.BuildingBlocks.MongoDb;
namespace Vogel.Social.MongoEntities.Friendship
{
    [MongoCollection(FriendshipMongoConsts.FriendCollection)]
    public class FriendMongoEntity : FullAuditedMongoEntity<string>
    {
        public string SourceId { get; set; }
        public string TargetId { get; set; }

    }
}

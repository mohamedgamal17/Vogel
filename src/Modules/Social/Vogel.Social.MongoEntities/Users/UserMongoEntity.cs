using Vogel.BuildingBlocks.MongoDb;
using Vogel.Social.Shared.Common;
namespace Vogel.Social.MongoEntities.Users
{
    [MongoCollection(UserMongoConsts.CollectionName)]
    public class UserMongoEntity : FullAuditedMongoEntity<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string? AvatarId { get; set; }
    }
}

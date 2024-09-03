using Vogel.BuildingBlocks.MongoDb;
using Vogel.Social.MongoEntities.Pictures;
using Vogel.Social.Shared.Common;

namespace Vogel.Social.MongoEntities.Users
{
    public class UserMongoView : FullAuditedMongoEntity<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string? AvatarId { get; set; }
        public PictureMongoEntity? Avatar { get; set; }
    }
}

using Vogel.BuildingBlocks.MongoDb;
using Vogel.MongoDb.Entities.Medias;

namespace Vogel.MongoDb.Entities.Users
{
    public class UserMongoView : FullAuditedMongoEntity<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string? AvatarId { get; set; }
        public MediaMongoEntity? Avatar { get; set; }
    }
}

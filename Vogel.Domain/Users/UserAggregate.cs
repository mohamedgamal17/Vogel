using Vogel.BuildingBlocks.Domain;
using Vogel.BuildingBlocks.Domain.Auditing;
using Vogel.Domain.Medias;

namespace Vogel.Domain.Users
{
    public class UserAggregate : AuditedAggregateRoot<string>
    {
        public UserAggregate()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string? AvatarId { get; set; }
        public Media? Avatar { get; set; }
    }
}

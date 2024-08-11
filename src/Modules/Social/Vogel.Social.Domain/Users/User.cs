using Vogel.BuildingBlocks.Domain.Auditing;

namespace Vogel.Domain.Users
{
    public class User : AuditedAggregateRoot<string>
    {
        public User(string id)
        {
            Id = id;
        }
        public User()
        {
            Id = Guid.NewGuid().ToString();
        }     
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string? AvatarId { get; set; }
    }
}

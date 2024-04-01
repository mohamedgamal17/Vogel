
namespace Vogel.Domain
{
    public class PublicUserView : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string? AvatarId { get; set; }
        public Media Avatar { get; set; }
        protected override string GetEntityPerfix()
        {
            return string.Empty;
        }
    }
}

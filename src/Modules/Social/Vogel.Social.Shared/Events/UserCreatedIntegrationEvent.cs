using Vogel.Social.Shared.Common;
using Vogel.Social.Shared.Events.Models;

namespace Vogel.Social.Shared.Events
{
    public class UserCreatedIntegrationEvent
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string? AvatarId { get; set; }
        public PictureModel? Avatar { get; set; }
    }
}

using Vogel.Application.Common.Dtos;
using Vogel.Application.Medias.Dtos;
using Vogel.Domain;

namespace Vogel.Application.Users.Dtos
{
    public class UserAggregateDto : EntityDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string? AvatarId { get; set; }
        public MediaAggregateDto? Avatar { get; set; }
    }
}

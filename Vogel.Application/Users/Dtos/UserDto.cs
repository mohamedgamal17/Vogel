using Vogel.Application.Common.Dtos;
using Vogel.Domain;

namespace Vogel.Application.Users.Dtos
{
    public class UserDto : EntityDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
    }
}

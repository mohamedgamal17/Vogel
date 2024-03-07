using Vogel.Application.Common.Interfaces;
using Vogel.Application.Users.Dtos;
using Vogel.Domain;

namespace Vogel.Application.Users.Commands
{
    public abstract class UserCommandBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string MediaId { get; set; }
    }

    public class CreateUserCommand : UserCommandBase , ICommand<UserDto> { }

    public class UpdateUserCommand : UserCommandBase , ICommand<UserDto> 
    {
        public string Id { get; set; }
    }
}

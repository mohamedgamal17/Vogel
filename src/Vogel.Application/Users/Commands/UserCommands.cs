using Microsoft.AspNetCore.Authorization;
using Vogel.Application.Users.Dtos;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Domain.Users;
namespace Vogel.Application.Users.Commands
{
    public abstract class UserCommandBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string? AvatarId { get; set; }
    }

    [Authorize]
    public class CreateUserCommand : UserCommandBase , ICommand<UserDto> { }

    [Authorize]
    public class UpdateUserCommand : UserCommandBase , ICommand<UserDto> 
    {
    }
}

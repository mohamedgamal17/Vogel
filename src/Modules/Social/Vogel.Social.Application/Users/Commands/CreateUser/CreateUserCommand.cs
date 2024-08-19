using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Social.Application.Users.Commands.CreateUser
{
    [Authorize]
    public class CreateUserCommand : ICommand<UserDto>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Domain.Users.Gender Gender { get; set; }
        public string? AvatarId { get; set; }
    }
}

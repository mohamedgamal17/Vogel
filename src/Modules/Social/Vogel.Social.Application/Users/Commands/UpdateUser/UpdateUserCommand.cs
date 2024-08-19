using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommand : ICommand<UserDto>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Domain.Users.Gender Gender { get; set; }
        public string? AvatarId { get; set; }
    }
}

using Vogel.Application.Comments.Commands;
using Vogel.Application.Users.Commands;
using Vogel.Domain;

namespace Vogel.Host.Models
{
    public class UserModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string? AvatarId { get; set; }

        public CreateUserCommand ToCreateUserCommand()
        {
            var command = new CreateUserCommand
            {
                FirstName = FirstName,
                LastName = LastName,
                AvatarId = AvatarId,
                Gender= Gender,
                BirthDate = BirthDate
            };

            return command;
        }

        public UpdateUserCommand ToUpdateUserCommand()
        {
            var command = new UpdateUserCommand
            {
                FirstName = FirstName,
                LastName = LastName,
                Gender = Gender,
                AvatarId = AvatarId,
                BirthDate = BirthDate
            };

            return command;
        }
    }
}

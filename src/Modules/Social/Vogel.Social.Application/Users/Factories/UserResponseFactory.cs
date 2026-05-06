using Vogel.Social.MongoEntities.Users;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Users.Factories
{
    public class UserResponseFactory : IUserResponseFactory
    {
        public async Task<List<UserDto>> PrepareListUserDto(List<UserMongoView> users)
        {
            var tasks = users.Select(PrepareUserDto);

            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        public async Task<UserDto> PrepareUserDto(UserMongoView user)
        {
            var result = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                BirthDate = user.BirthDate.ToShortDateString()
            };

            return result;
        }
    }
}
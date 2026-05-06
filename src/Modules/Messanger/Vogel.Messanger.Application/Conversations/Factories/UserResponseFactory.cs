using Vogel.Messanger.MongoEntities.Users;
using Vogel.Messanger.Application.Conversations.Dtos;

namespace Vogel.Messanger.Application.Conversations.Factories
{
    public class UserResponseFactory : IUserResponseFactory
    {
        public async Task<List<ConversationUserDto>> PrepareListUserDto(List<UserMongoEntity> users)
        {
            var userTasks = users.Select(PreapreUserDto);

            var results = await Task.WhenAll(userTasks);

            return results.ToList();
        }

        public async Task<ConversationUserDto> PreapreUserDto(UserMongoEntity user)
        {
            var dto = new ConversationUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                BirthDate = user.BirthDate.ToShortDateString(),
            };

            if(user.Avatar != null)
            {
                dto.Avatar = user.Avatar.Reference;
            }

            return dto;
        }

    }


}

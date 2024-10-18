using Vogel.Messanger.MongoEntities.Users;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Messanger.Application.Conversations.Factories
{
    public interface IUserResponseFactory
    {
        Task<List<UserDto>> PrepareListUserDto(List<UserMongoEntity> users);
        Task<UserDto> PreapreUserDto(UserMongoEntity user);
    }

}

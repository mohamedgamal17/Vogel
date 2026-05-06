using Vogel.BuildingBlocks.Application.Factories;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.MongoEntities.Users;

namespace Vogel.Messanger.Application.Conversations.Factories
{
    public interface IUserResponseFactory : IResponseFactory
    {
        Task<List<ConversationUserDto>> PrepareListUserDto(List<UserMongoEntity> users);
        Task<ConversationUserDto> PreapreUserDto(UserMongoEntity user);
    }

}

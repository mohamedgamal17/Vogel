using Vogel.BuildingBlocks.Application.Factories;
using Vogel.Social.MongoEntities.Users;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Users.Factories
{
    public interface IUserResponseFactory : IResponseFactory
    {
        Task<List<UserDto>> PrepareListUserDto(List<UserMongoView> users);
        Task<UserDto> PrepareUserDto(UserMongoView user);
    }
}
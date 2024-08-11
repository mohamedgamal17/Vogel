using Vogel.Application.Users.Dtos;
using Vogel.BuildingBlocks.Application.Factories;
using Vogel.Domain.Medias;
using Vogel.Domain.Users;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.Application.Users.Factories
{
    public interface IUserResponseFactory : IResponseFactory
    {
        Task<List<UserDto>> PrepareListUserDto(List<UserMongoView> users);
        Task<UserDto> PrepareUserDto(UserMongoView user);
    }
}

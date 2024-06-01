using Vogel.Application.Users.Dtos;
using Vogel.BuildingBlocks.Application.Factories;
using Vogel.Domain.Medias;
using Vogel.Domain.Users;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.Application.Users.Factories
{
    public interface IUserResponseFactory : IResponseFactory
    {
        Task<List<UserDto>> PrepareListUserAggregateDto(List<UserMongoView> users);
        Task<UserDto> PrepareUserAggregateDto(UserAggregate user, Media? avatar = null);
        Task<UserDto> PrepareUserAggregateDto(UserMongoView user);
        Task<PublicUserDto> PreparePublicUserDto(PublicUserMongoView user);
    }
}

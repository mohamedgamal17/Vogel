using Vogel.Application.Friendship.Dtos;
using Vogel.BuildingBlocks.Application.Factories;
using Vogel.Domain.Friendship;
using Vogel.MongoDb.Entities.Friendship;

namespace Vogel.Application.Friendship.Factories
{
    public interface IFriendshipResponseFactory : IResponseFactory
    {

        Task<List<FriendRequestDto>> PrepareListFriendRequestDto(List<FriendRequestMongoView> friendRequests);

        Task<FriendRequestDto> PrepareFriendRequestDto(FriendRequestMongoView friendRequest);

        Task<List<FriendDto>> PrepareListFriendDto(List<FriendMongoView> friends);

        Task<FriendDto> PrepareFriendDto(FriendMongoView friend);
    }
}

using Vogel.BuildingBlocks.Application.Factories;
using Vogel.Social.MongoEntities.Friendship;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Social.Application.Friendship.Factories
{
    public interface IFriendshipResponseFactory : IResponseFactory
    {

        Task<List<FriendRequestDto>> PrepareListFriendRequestDto(List<FriendRequestMongoView> friendRequests);

        Task<FriendRequestDto> PrepareFriendRequestDto(FriendRequestMongoView friendRequest);

        Task<List<FriendDto>> PrepareListFriendDto(List<FriendMongoView> friends);

        Task<FriendDto> PrepareFriendDto(FriendMongoView friend);
    }
}

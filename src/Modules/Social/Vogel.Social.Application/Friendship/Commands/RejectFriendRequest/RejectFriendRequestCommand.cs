using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Social.Application.Friendship.Commands.RejectFriendRequest
{
    [Authorize]
    public class RejectFriendRequestCommand : ICommand<FriendRequestDto>
    {
        public string FriendRequestId { get; set; }
    }
}

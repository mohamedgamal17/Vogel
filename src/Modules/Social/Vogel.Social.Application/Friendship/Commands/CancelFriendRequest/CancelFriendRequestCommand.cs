using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Friendship.Commands.CancelFriendRequest
{
    [Authorize]
    public class CancelFriendRequestCommand : ICommand<FriendRequestDto>
    {
        public string FriendRequestId { get; set; }
    }
}

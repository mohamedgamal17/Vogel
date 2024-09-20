using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Friendship.Commands.AcceptFriendRequest
{
    [Authorize]
    public class AcceptFriendRequestCommand : ICommand<FriendDto>
    {
        public string FriendRequestId { get; set; }
    }
}

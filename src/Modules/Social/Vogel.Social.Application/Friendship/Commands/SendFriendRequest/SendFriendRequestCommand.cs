using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Friendship.Commands.SendFriendRequest
{
    [Authorize]
    public class SendFriendRequestCommand : ICommand<FriendRequestDto>
    {
        public string ReciverId { get; set; }
    }
}

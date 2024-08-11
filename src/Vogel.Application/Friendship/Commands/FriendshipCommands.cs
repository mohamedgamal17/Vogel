using Microsoft.AspNetCore.Authorization;
using Vogel.Application.Friendship.Dtos;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Domain.Friendship;

namespace Vogel.Application.Friendship.Commands
{
    [Authorize]
    public class SendFriendRequestCommand : ICommand<FriendRequestDto>
    {
        public string ReciverId { get; set; }
    }

    [Authorize]
    public class AcceptFriendRequestCommand : ICommand<FriendDto>
    {
        public string FriendRequestId { get; set; }
    }

    [Authorize]
    public class RejectFriendRequestCommand : ICommand<FriendRequestDto>
    {
        public string FriendRequestId { get; set; }
    }

    [Authorize]
    public class CancelFriendRequestCommand : ICommand<FriendRequestDto>
    {
        public string FriendRequestId { get; set; }     
    }

    [Authorize]
    public class RemoveFriendCommand : ICommand
    {
        public string Id { get; set; }
    }
}

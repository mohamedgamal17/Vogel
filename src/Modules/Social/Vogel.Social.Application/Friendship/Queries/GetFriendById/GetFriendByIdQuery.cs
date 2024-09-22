using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Friendship.Queries.GetFriendById
{
    [Authorize]
    public class GetFriendByIdQuery : IQuery<FriendDto>
    {
        public string FriendId { get; set; }
    }
}

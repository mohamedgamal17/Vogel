using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Friendship.Queries.ListFriendRequest
{
    [Authorize]
    public class ListFriendRequestQuery : PagingParams, IQuery<Paging<FriendRequestDto>>
    {
        public string UserId { get; set; }
    }
}

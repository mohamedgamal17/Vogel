using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Friendship.Queries.ListFriendRequest
{
    public class ListFriendRequestQuery : PagingParams, IQuery<Paging<FriendRequestDto>>
    {
        public string UserId { get; set; }
    }
}

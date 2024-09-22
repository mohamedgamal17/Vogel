using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Friendship.Queries.ListFriends
{
    public class ListFriendsQuery : PagingParams, IQuery<Paging<FriendDto>>
    {
        public string UserId { get; set; }
    }
}

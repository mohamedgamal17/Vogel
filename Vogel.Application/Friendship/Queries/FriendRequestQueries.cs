using Vogel.Application.Common.Models;
using Vogel.Application.Friendship.Dtos;
using Vogel.BuildingBlocks.Application.Requests;

namespace Vogel.Application.Friendship.Queries
{
    public class ListFriendRequestQuery : PagingParams, IQuery<Paging<FriendRequestDto>>
    {

    }

    public class GetFriendRequestByIdQuery : IQuery<FriendRequestDto>
    {
        public string Id { get; set; }
    }

}

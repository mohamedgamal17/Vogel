using Vogel.Application.Common.Models;
using Vogel.Application.Friendship.Dtos;
using Vogel.BuildingBlocks.Application.Requests;

namespace Vogel.Application.Friendship.Queries
{
    public class ListFriendQuery : PagingParams , IQuery<Paging<FriendDto>>
    {

    }

    public class GetFriendByIdQuery : IQuery<FriendDto>
    {
        public string Id { get; set; }
    }
}

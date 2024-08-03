using Vogel.Application.Common.Models;
using Vogel.Application.Friendship.Dtos;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.MongoDb.Entities.Common;

namespace Vogel.Application.Friendship.Queries
{
    public class ListFriendRequestQuery : PagingParams, IQuery<Paging<FriendRequestDto>>
    {
        public string UserId { get; set; }
    }

    public class GetFriendRequestByIdQuery : IQuery<FriendRequestDto>
    {
        public string Id { get; set; }
    }

}

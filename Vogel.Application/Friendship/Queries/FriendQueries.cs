using Vogel.Application.Common.Models;
using Vogel.Application.Friendship.Dtos;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.MongoDb.Entities.Common;

namespace Vogel.Application.Friendship.Queries
{
    public class ListFriendQuery : PagingParams , IQuery<Paging<FriendDto>>
    {
        public string UserId { get; set; }
    }

    public class GetFriendByIdQuery : IQuery<FriendDto>
    {
        public string Id { get; set; }
    }
}

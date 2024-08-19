using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Users.Queries.ListUsers
{
    [Authorize]
    public class ListUsersQuery : PagingParams ,IQuery<Paging<UserDto>>
    {
    }
}

using Microsoft.AspNetCore.Authorization;
using Vogel.Application.Common.Models;
using Vogel.Application.Users.Dtos;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.MongoDb.Models;

namespace Vogel.Social.Shared.Users.Queries
{
    [Authorize]
    public class ListUserQuery : PagingParams , IQuery<Paging<UserDto>>
    {
    }
}

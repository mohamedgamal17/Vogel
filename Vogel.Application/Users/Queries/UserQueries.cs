using Microsoft.AspNetCore.Authorization;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Common.Models;
using Vogel.Application.Users.Dtos;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.MongoDb.Entities.Common;
namespace Vogel.Application.Users.Queries
{
    [Authorize]
    public class ListUserQuery :PagingParams, IQuery<Paging<UserDto>> { }

    [Authorize]
    public class GetCurrentUserQuery : IQuery<UserDto> { }

    [Authorize]
    public class GetUserByIdQuery : IQuery<UserDto>
    {
        public string Id { get; set; }
    }

    [Authorize]
    public class SearchOnUserByNameQuery :PagingParams ,IQuery<Paging<UserDto>>
    {
        public string Name { get; set; }
    }
}

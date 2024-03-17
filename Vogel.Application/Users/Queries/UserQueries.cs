using Microsoft.AspNetCore.Authorization;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Common.Models;
using Vogel.Application.Users.Dtos;

namespace Vogel.Application.Users.Queries
{
    [Authorize]
    public class ListUserQuery :PagingParams, IQuery<Paging<UserAggregateDto>> { }

    [Authorize]
    public class GetCurrentUserQuery : IQuery<UserAggregateDto> { }

    [Authorize]
    public class GetUserByIdQuery : IQuery<UserAggregateDto>
    {
        public string Id { get; set; }
    }
}

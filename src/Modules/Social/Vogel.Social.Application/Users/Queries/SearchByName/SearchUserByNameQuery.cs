using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Users.Queries.SearchByName
{
    [Authorize]
    public class SearchUserByNameQuery : PagingParams, IQuery<Paging<UserDto>>
    {
        public string Name { get; set; }
    }
}

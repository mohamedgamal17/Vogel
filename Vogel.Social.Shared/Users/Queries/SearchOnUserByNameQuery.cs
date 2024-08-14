using Microsoft.AspNetCore.Authorization;
using Vogel.Application.Users.Dtos;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.MongoDb.Models;
namespace Vogel.Social.Shared.Users.Queries
{
    [Authorize]
    public class SearchOnUserByNameQuery : IQuery<Paging<UserDto>>
    {
        public string Name { get; set; }
    }
}

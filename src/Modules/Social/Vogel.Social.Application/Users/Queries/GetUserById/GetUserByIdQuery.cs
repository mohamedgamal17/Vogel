using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Users.Queries.GetUserById
{
    [Authorize]
    public class GetUserByIdQuery :  IQuery<UserDto>
    {
        public string Id { get; set; }
    }

}

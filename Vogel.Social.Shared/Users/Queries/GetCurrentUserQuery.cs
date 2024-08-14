using Microsoft.AspNetCore.Authorization;
using Vogel.Application.Users.Dtos;
using Vogel.BuildingBlocks.Application.Requests;

namespace Vogel.Social.Shared.Users.Queries
{
    [Authorize]
    public class GetCurrentUserQuery : IQuery<UserDto>
    {
    }
}

using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Users.Queries.GetCurrentUser
{
    [Authorize]
    public class GetCurrentUserQuery : IQuery<UserDto>
    {
    }
}

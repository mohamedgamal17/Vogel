
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Vogel.Domain.Medias;

namespace Vogel.Application.Medias.Policies
{
    public class IsMediaOwnerAuthorizationHandler : AuthorizationHandler<IsMediaOwnerRequirment, Media>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsMediaOwnerRequirment requirement, Media resource)
        {
            string userId = context.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if(userId == resource.UserId)
            {
                  context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}

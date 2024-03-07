
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using MongoDB.Driver.Core.Authentication;
using System.Security.Claims;
using Vogel.Domain;

namespace Vogel.Application.Posts.Policies
{
    public class PostOperationAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Post>
    {

        private readonly ISecurityContext _securityContext;

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Post resource)
        {
            if(requirement.Name == PostOperationAuthorizationRequirement.Create.Name)
            {
                context.Succeed(requirement);
            }
            else
            {
                string userId = context.User.Claims
                    .Single(x => x.Type == ClaimTypes.NameIdentifier).Value;

                if (userId == resource.UserId)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}

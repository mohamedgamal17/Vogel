using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.Domain.Posts;

namespace Vogel.Content.Application.PostReactions.Policies
{
    public class IsPostReactionOwnerAuthorizationRequirment : IAuthorizationRequirement
    {
    }

    public class IsPostReactionOwnerAuthorizationRequirmentHandler : AuthorizationHandler<IsPostReactionOwnerAuthorizationRequirment, PostReaction>
    {
        private readonly ISecurityContext _securityContext;

        public IsPostReactionOwnerAuthorizationRequirmentHandler(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsPostReactionOwnerAuthorizationRequirment requirement, PostReaction resource)
        {
            string userId = _securityContext.User!.Id!;

            if (userId == resource.UserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.Domain.Friendship;

namespace Vogel.Application.Friendship.Policies
{
    public class FriendRequestReciverActionAuthorizationRequirment : IAuthorizationRequirement
    {
     
    }

    public class AcceptFriendRequestAuthorizationRequirmentHandler : AuthorizationHandler<FriendRequestReciverActionAuthorizationRequirment, FriendRequest>
    {
        private readonly ISecurityContext _securityContext;
        public AcceptFriendRequestAuthorizationRequirmentHandler(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FriendRequestReciverActionAuthorizationRequirment requirement, FriendRequest resource)
        {
            if(resource.ReciverId == _securityContext.User!.Id)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

}

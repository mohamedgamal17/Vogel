using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.Social.Domain.Friendship;
namespace Vogel.Social.Application.Friendship.Policies
{
    public class FriendRequestSenderActionRequirment : IAuthorizationRequirement
    {
    }

    public class SenderActionFriendRequestRequirmentHandler : AuthorizationHandler<FriendRequestSenderActionRequirment, FriendRequest>
    {
        private readonly ISecurityContext _securityContext;

        public SenderActionFriendRequestRequirmentHandler(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FriendRequestSenderActionRequirment requirement, FriendRequest resource)
        {
            if (resource.SenderId == _securityContext.User!.Id)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}

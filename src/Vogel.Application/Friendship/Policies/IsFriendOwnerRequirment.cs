using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.Domain.Friendship;
using Vogel.MongoDb.Entities.Friendship;

namespace Vogel.Application.Friendship.Policies
{
    public class IsFriendOwnerRequirment : IAuthorizationRequirement
    {
    }

    public class IsFriendOwnerRequirmentHandler : AuthorizationHandler<IsFriendOwnerRequirment, Friend>
    {
        private readonly ISecurityContext _securityContext;

        public IsFriendOwnerRequirmentHandler(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsFriendOwnerRequirment requirement, Friend resource)
        {
            if(resource.SourceId == _securityContext.User!.Id || 
                resource.TargetId == _securityContext.User!.Id)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class IsFriendMongoViewOwnerRequirmentHandler : AuthorizationHandler<IsFriendOwnerRequirment, FriendMongoView>
    {
        private readonly ISecurityContext _securityContext;

        public IsFriendMongoViewOwnerRequirmentHandler(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsFriendOwnerRequirment requirement, FriendMongoView resource)
        {
            if (resource.SourceId == _securityContext.User!.Id ||
                resource.TargetId == _securityContext.User!.Id)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}

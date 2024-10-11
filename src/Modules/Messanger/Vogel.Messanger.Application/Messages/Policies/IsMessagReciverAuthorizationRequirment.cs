using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.Messanger.Domain.Messages;

namespace Vogel.Messanger.Application.Messages.Policies
{
    public class IsMessagReciverAuthorizationRequirment : IAuthorizationRequirement
    {

    }

    public class MessagReciverAuthorizationRequirmentHandler : AuthorizationHandler<IsMessagReciverAuthorizationRequirment, Message>
    {
        private readonly ISecurityContext _securityContext;

        public MessagReciverAuthorizationRequirmentHandler(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsMessagReciverAuthorizationRequirment requirement, Message resource)
        {
            if(resource.ReciverId == _securityContext.User!.Id)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}

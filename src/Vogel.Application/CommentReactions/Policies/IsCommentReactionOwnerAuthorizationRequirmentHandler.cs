using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.Domain.Comments;

namespace Vogel.Application.CommentReactions.Policies
{
    public class IsCommentReactionOwnerAuthorizationRequirment : IAuthorizationRequirement
    {

    }
    public class IsCommentReactionOwnerAuthorizationRequirmentHandler : AuthorizationHandler<IsCommentReactionOwnerAuthorizationRequirment, CommentReaction>
    {
        public ISecurityContext _securityContext;

        public IsCommentReactionOwnerAuthorizationRequirmentHandler(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsCommentReactionOwnerAuthorizationRequirment requirement, CommentReaction resource)
        {
            if(resource.UserId == _securityContext.User!.Id)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}

using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Vogel.Content.Application.Comments.Polices
{
    public static class CommentOperationAuthorizationRequirement
    {

        public static RemoveCommmentAuthorizationRequirment Remove = new RemoveCommmentAuthorizationRequirment();
       
    }
}

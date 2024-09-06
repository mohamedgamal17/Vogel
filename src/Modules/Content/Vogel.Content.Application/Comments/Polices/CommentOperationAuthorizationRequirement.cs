
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Vogel.Content.Application.Comments.Polices
{
    public static class CommentOperationAuthorizationRequirement
    {
        public static OperationAuthorizationRequirement Edit = new OperationAuthorizationRequirement
        {
            Name = $"Comment_{nameof(Edit)}"
        };

        public static OperationAuthorizationRequirement Delete = new OperationAuthorizationRequirement
        {
            Name = $"Comment_{nameof(Delete)}"
        };
    }



}

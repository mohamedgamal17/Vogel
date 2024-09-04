using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Vogel.Content.Application.Posts.Policies
{
    public static class PostOperationAuthorizationRequirement
    {

        public static OperationAuthorizationRequirement Create =
            new OperationAuthorizationRequirement { Name = $"Post_{nameof(Create)}" };

        public static OperationAuthorizationRequirement Edit =
           new OperationAuthorizationRequirement { Name = $"Post_{nameof(Edit)}" };

        public static OperationAuthorizationRequirement Delete =
                new OperationAuthorizationRequirement { Name = $"Post_{nameof(Delete)}" };
    }
}

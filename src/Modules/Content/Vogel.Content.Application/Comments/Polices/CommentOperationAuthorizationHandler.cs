using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Security.Claims;
using Vogel.Content.Domain.Comments;
using Vogel.Content.MongoEntities.Posts;
namespace Vogel.Content.Application.Comments.Polices
{
    public class CommentOperationAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Comment>
    {
        private readonly PostMongoRepository _postMongoRepository;

        public CommentOperationAuthorizationHandler(PostMongoRepository postMongoRepository)
        {
            _postMongoRepository = postMongoRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Comment resource)
        {
            if (requirement.Name == CommentOperationAuthorizationRequirement.Edit.Name)
            {
                if (IsResourceOwner(context, resource))
                {
                    context.Succeed(requirement);
                }
            }
            else
            {
                if (IsResourceOwner(context, resource))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    var post = await _postMongoRepository.FindByIdAsync(resource.PostId)!;

                    string userId = context.User.Claims
                    .Single(x => x.Type == ClaimTypes.NameIdentifier).Value;

                    if (post.UserId == userId)
                    {
                        context.Succeed(requirement);
                    }
                }
            }
        }


        private bool IsResourceOwner(AuthorizationHandlerContext context, Comment resource)
        {
            string userId = context.User.Claims
                    .Single(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (resource.UserId != userId)
            {
                return false;
            }

            return true;
        }
    }
}

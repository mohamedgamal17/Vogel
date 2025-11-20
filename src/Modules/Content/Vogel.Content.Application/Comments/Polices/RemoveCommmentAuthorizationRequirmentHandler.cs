using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.Content.Domain.Comments;
using Vogel.Content.MongoEntities.Comments;
using Vogel.Content.MongoEntities.Posts;
namespace Vogel.Content.Application.Comments.Polices
{
    public class RemoveCommmentAuthorizationRequirmentHandler : AuthorizationHandler<RemoveCommmentAuthorizationRequirment, Comment>
    {
        private readonly PostMongoRepository _postMongoRepository;
        private readonly CommentMongoRepository _commentMongoRepository;
        private readonly ISecurityContext _securityContext;

        public RemoveCommmentAuthorizationRequirmentHandler(PostMongoRepository postMongoRepository, CommentMongoRepository commentMongoRepository, ISecurityContext securityContext)
        {
            _postMongoRepository = postMongoRepository;
            _commentMongoRepository = commentMongoRepository;
            _securityContext = securityContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RemoveCommmentAuthorizationRequirment requirement, Comment resource)
        {
            string userId = _securityContext.User!.Id;

            if (resource.IsOwnedBy(userId))
            {
                context.Succeed(requirement);

                return;
            }

            if (resource.CommentId != null)
            {
                var parentComment = await _commentMongoRepository.SingleAsync(x => x.Id == resource.CommentId);

                if (parentComment.IsOwnedBy(userId))
                {
                    context.Succeed(requirement);

                    return;
                }
            }
            else
            {
                var post = await _postMongoRepository.SingleAsync(x => x.Id == resource.PostId);

                if (post.IsOwnedBy(userId))
                {
                    context.Succeed(requirement);

                }
            }
        }

    }
}

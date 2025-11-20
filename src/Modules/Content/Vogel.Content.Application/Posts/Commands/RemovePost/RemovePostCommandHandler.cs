using MediatR;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Posts;
namespace Vogel.Content.Application.Posts.Commands.RemovePost
{
    public class RemovePostCommandHandler : IApplicationRequestHandler<RemovePostCommand, Unit>
    {
        private readonly IContentRepository<Post> _postRepository;
        private readonly ISecurityContext _securityContext;

        public RemovePostCommandHandler(IContentRepository<Post> postRepository, ISecurityContext securityContext)
        {
            _postRepository = postRepository;
            _securityContext = securityContext;
        }

        public async Task<Result<Unit>> Handle(RemovePostCommand request, CancellationToken cancellationToken)
        {
            string userId = _securityContext.User!.Id;

            var post = await _postRepository.FindByIdAsync(request.PostId);

            if (post == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(Post), request.PostId));
            }

            if (!post.IsOwnedBy(userId))
            {
                return new Result<Unit>(new ForbiddenAccessException());
            }

            await _postRepository.DeleteAsync(post);

            return Unit.Value;
        }
    }
}

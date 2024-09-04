using MediatR;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.Posts.Policies;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Posts;
namespace Vogel.Content.Application.Posts.Commands.RemovePost
{
    public class RemovePostCommandHandler : IApplicationRequestHandler<RemovePostCommand, Unit>
    {
        private readonly IContentRepository<Post> _postRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public RemovePostCommandHandler(IContentRepository<Post> postRepository, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _postRepository = postRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<Unit>> Handle(RemovePostCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.FindByIdAsync(request.PostId);

            if (post == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(Post), request.PostId));
            }

            var authorizationResult = await _applicationAuthorizationService
                .AuthorizeAsync(post, PostOperationAuthorizationRequirement.Edit);

            if (authorizationResult.IsFailure)
            {
                return authorizationResult;
            }

            await _postRepository.DeleteAsync(post);

            return Unit.Value;
        }
    }
}

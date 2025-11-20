using MediatR;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Posts;
namespace Vogel.Content.Application.PostReactions.Commands.RemovePostReaction
{
    public class RemovePostReactionCommandHandler : IApplicationRequestHandler<RemovePostReactionCommand, Unit>
    {
        private readonly IContentRepository<PostReaction> _postReactionRepository;
        private readonly ISecurityContext _securityContext;

        public RemovePostReactionCommandHandler(IContentRepository<PostReaction> postReactionRepository, ISecurityContext securityContext)
        {
            _postReactionRepository = postReactionRepository;
            _securityContext = securityContext;
        }

        public async Task<Result<Unit>> Handle(RemovePostReactionCommand request, CancellationToken cancellationToken)
        {
            string userId = _securityContext.User!.Id;

            var reaction = await _postReactionRepository.SingleOrDefaultAsync(x => x.Id == request.ReactionId && x.PostId == request.PostId);

            if (reaction == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(PostReaction), request.ReactionId));
            }

            if (!reaction.IsOwnedBy(userId))
            {
                return new Result<Unit>(new ForbiddenAccessException());
            }

            await _postReactionRepository.DeleteAsync(reaction);

            return Unit.Value;
        }
    }
}

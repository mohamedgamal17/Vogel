using MediatR;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.PostReactions.Factories;
using Vogel.Content.Application.PostReactions.Policies;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.PostReactions;
using Vogel.Social.Domain;
namespace Vogel.Content.Application.PostReactions.Commands.RemovePostReaction
{
    public class RemovePostReactionCommandHandler : IApplicationRequestHandler<RemovePostReactionCommand, Unit>
    {
        private readonly ISocialRepository<PostReaction> _postReactionRepository;
        private readonly IMongoRepository<PostReactionMongoEntity> _postReactionMongoRepository;
        private readonly IPostReactionResponseFactory _postReactionResponseFactory;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public RemovePostReactionCommandHandler(ISocialRepository<PostReaction> postReactionRepository, IMongoRepository<PostReactionMongoEntity> postReactionMongoRepository, IPostReactionResponseFactory postReactionResponseFactory, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _postReactionRepository = postReactionRepository;
            _postReactionMongoRepository = postReactionMongoRepository;
            _postReactionResponseFactory = postReactionResponseFactory;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<Unit>> Handle(RemovePostReactionCommand request, CancellationToken cancellationToken)
        {
            var reaction = await _postReactionRepository.SingleOrDefaultAsync(x => x.Id == request.ReactionId && x.PostId == request.PostId);

            if (reaction == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(PostReaction), request.ReactionId));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(reaction, PostReactionOperationAuthorizationRequirment.IsOwner);

            if (authorizationResult.IsFailure)
            {
                return new Result<Unit>(authorizationResult.Exception!);
            }

            await _postReactionRepository.DeleteAsync(reaction);

            return Unit.Value;
        }
    }
}

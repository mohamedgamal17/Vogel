using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.Content.Application.PostReactions.Factories;
using Vogel.Content.Application.PostReactions.Policies;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.PostReactions;
namespace Vogel.Content.Application.PostReactions.Commands.UpdatePostReaction
{
    public class UpdatePostReactionCommandHandler : IApplicationRequestHandler<UpdatePostReactionCommand, PostReactionDto>
    {
        private readonly IContentRepository<PostReaction> _postReactionRepository;
        private readonly IMongoRepository<PostReactionMongoEntity> _postReactionMongoRepository;
        private readonly IPostReactionResponseFactory _postReactionResponseFactory;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public UpdatePostReactionCommandHandler(IContentRepository<PostReaction> postReactionRepository, IMongoRepository<PostReactionMongoEntity> postReactionMongoRepository, IPostReactionResponseFactory postReactionResponseFactory, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _postReactionRepository = postReactionRepository;
            _postReactionMongoRepository = postReactionMongoRepository;
            _postReactionResponseFactory = postReactionResponseFactory;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<PostReactionDto>> Handle(UpdatePostReactionCommand request, CancellationToken cancellationToken)
        {
            var reaction = await _postReactionRepository.SingleOrDefaultAsync(x => x.Id == request.ReactionId && x.PostId == request.PostId);

            if (reaction == null)
            {
                return new Result<PostReactionDto>(new EntityNotFoundException(typeof(PostReaction), request.ReactionId));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(reaction, PostReactionOperationAuthorizationRequirment.IsOwner);

            if (authorizationResult.IsFailure)
            {
                return new Result<PostReactionDto>(authorizationResult.Exception!);
            }

            reaction.Type = request.Type;

            await _postReactionRepository.UpdateAsync(reaction);

            var mongoView = await _postReactionMongoRepository.FindByIdAsync(reaction.Id);

            return await _postReactionResponseFactory.PreparePostReactionDto(mongoView!);
        }
    }
}

using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.CommentReactions.Dtos;
using Vogel.Content.Application.CommentReactions.Factories;
using Vogel.Content.Application.CommentReactions.Policies;
using Vogel.Content.Domain.Comments;
using Vogel.Content.MongoEntities.CommentReactions;
using Vogel.Social.Domain;
namespace Vogel.Content.Application.CommentReactions.Commands.UpdateCommentReaction
{
    public class UpdateCommentReactionCommandHandler : IApplicationRequestHandler<UpdateCommentReactionCommand, CommentReactionDto>
    {
        private readonly ISocialRepository<CommentReaction> _commentReactionRepository;
        private readonly ISocialRepository<Comment> _commentRepository;
        private readonly IMongoRepository<CommentReactionMongoEntity> _commentReactionMongoRepository;
        private readonly ICommentReactionResponseFactory _commentReactionResponseFactory;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public UpdateCommentReactionCommandHandler(ISocialRepository<CommentReaction> commentReactionRepository, ISocialRepository<Comment> commentRepository, IMongoRepository<CommentReactionMongoEntity> commentReactionMongoRepository, ICommentReactionResponseFactory commentReactionResponseFactory, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _commentReactionRepository = commentReactionRepository;
            _commentRepository = commentRepository;
            _commentReactionMongoRepository = commentReactionMongoRepository;
            _commentReactionResponseFactory = commentReactionResponseFactory;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<CommentReactionDto>> Handle(UpdateCommentReactionCommand request, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.SingleOrDefaultAsync(x => x.Id == request.CommentId && x.PostId == request.PostId);

            if (comment == null)
            {
                return new Result<CommentReactionDto>(new EntityNotFoundException(typeof(Comment), request.CommentId));
            }

            var reaction = await _commentReactionRepository.SingleOrDefaultAsync(x => x.Id == request.ReactionId && x.CommentId == request.CommentId);

            if (reaction == null)
            {
                return new Result<CommentReactionDto>(new EntityNotFoundException(typeof(CommentReaction), request.ReactionId));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(reaction, CommentReactionOperationAuthorizationRequirment.IsOwner);

            if (authorizationResult.IsFailure)
            {
                return new Result<CommentReactionDto>(authorizationResult.Exception!);
            }


            reaction.Type = request.Type;

            await _commentReactionRepository.UpdateAsync(reaction);

            var reactionMongoEntity = await _commentReactionMongoRepository.FindByIdAsync(reaction.Id);

            return await _commentReactionResponseFactory.PrepareCommentReactionDto(reactionMongoEntity!);
        }
    }
}

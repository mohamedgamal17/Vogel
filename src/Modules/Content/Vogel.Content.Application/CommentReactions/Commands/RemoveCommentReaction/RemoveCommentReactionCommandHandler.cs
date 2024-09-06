using MediatR;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.CommentReactions.Policies;
using Vogel.Content.Domain.Comments;
using Vogel.Social.Domain;
namespace Vogel.Content.Application.CommentReactions.Commands.RemoveCommentReaction
{
    public class RemoveCommentReactionCommandHandler : IApplicationRequestHandler<RemoveCommentReactionCommand, Unit>
    {
        private readonly ISocialRepository<CommentReaction> _commentReactionRepository;
        private readonly ISocialRepository<Comment> _commentRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        public RemoveCommentReactionCommandHandler(ISocialRepository<CommentReaction> commentReactionRepository, ISocialRepository<Comment> commentRepository,  IApplicationAuthorizationService applicationAuthorizationService)
        {
            _commentReactionRepository = commentReactionRepository;
            _commentRepository = commentRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<Unit>> Handle(RemoveCommentReactionCommand request, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.SingleOrDefaultAsync(x => x.Id == request.CommentId && x.PostId == request.PostId);

            if (comment == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(Comment), request.CommentId));
            }

            var reaction = await _commentReactionRepository.SingleOrDefaultAsync(x => x.Id == request.ReactionId && x.CommentId == request.CommentId);

            if (reaction == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(CommentReaction), request.ReactionId));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(reaction, CommentReactionOperationAuthorizationRequirment.IsOwner);

            if (authorizationResult.IsFailure)
            {
                return new Result<Unit>(authorizationResult.Exception!);
            }

            await _commentReactionRepository.DeleteAsync(reaction);

            return Unit.Value;
        }
    }
}

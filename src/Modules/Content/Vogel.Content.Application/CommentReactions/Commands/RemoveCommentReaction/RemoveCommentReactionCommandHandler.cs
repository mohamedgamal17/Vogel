using MediatR;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Comments;
namespace Vogel.Content.Application.CommentReactions.Commands.RemoveCommentReaction
{
    public class RemoveCommentReactionCommandHandler : IApplicationRequestHandler<RemoveCommentReactionCommand, Unit>
    {
        private readonly IContentRepository<CommentReaction> _commentReactionRepository;
        private readonly IContentRepository<Comment> _commentRepository;
        private readonly ISecurityContext _securitContext;

        public RemoveCommentReactionCommandHandler(IContentRepository<CommentReaction> commentReactionRepository, IContentRepository<Comment> commentRepository, ISecurityContext securitContext)
        {
            _commentReactionRepository = commentReactionRepository;
            _commentRepository = commentRepository;
            _securitContext = securitContext;
        }

        public async Task<Result<Unit>> Handle(RemoveCommentReactionCommand request, CancellationToken cancellationToken)
        {
            string userId = _securitContext.User!.Id;

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

            if (!reaction.IsOwnedBy(userId))
            {
                return new Result<Unit>(new ForbiddenAccessException());
            }

            await _commentReactionRepository.DeleteAsync(reaction);

            return Unit.Value;
        }
    }
}

using MediatR;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.Comments.Polices;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Comments;
namespace Vogel.Content.Application.Comments.Commands.RemoveComent
{
    public class RemoveCommentCommandHandler : IApplicationRequestHandler<RemoveCommentCommand, Unit>
    {
        private readonly IContentRepository<Comment> _commentRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public RemoveCommentCommandHandler(IContentRepository<Comment> commentRepository,IApplicationAuthorizationService applicationAuthorizationService)
        {
            _commentRepository = commentRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<Unit>> Handle(RemoveCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.SingleOrDefaultAsync(x => x.Id == request.CommentId && x.PostId == request.PostId);

            if (comment == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(Comment), request.CommentId));
            }
            var authorizationResult = await _applicationAuthorizationService
                  .AuthorizeAsync(comment, CommentOperationAuthorizationRequirement.Delete);

            if (authorizationResult.IsFailure)
            {
                return authorizationResult!;
            }

            await _commentRepository.DeleteAsync(comment);

            return Unit.Value;
        }
    }
}

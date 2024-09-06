using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.Comments.Dtos;
using Vogel.Content.Application.Comments.Factories;
using Vogel.Content.Application.Comments.Polices;
using Vogel.Content.Domain.Comments;
using Vogel.Content.MongoEntities.Comments;
using Vogel.Social.Domain;

namespace Vogel.Content.Application.Comments.Commands.UpdateComment
{
    public class UpdateCommentCommandHandler : IApplicationRequestHandler<UpdateCommentCommand, CommentDto>
    {
        private readonly ISocialRepository<Comment> _commentRepository;
        private readonly ICommentResponseFactory _commentResponseFactory;
        private readonly CommentMongoRepository _commentMongoRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public UpdateCommentCommandHandler(ISocialRepository<Comment> commentRepository,  ICommentResponseFactory commentResponseFactory, CommentMongoRepository commentMongoRepository, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _commentRepository = commentRepository;
            _commentResponseFactory = commentResponseFactory;
            _commentMongoRepository = commentMongoRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<CommentDto>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _commentRepository.SingleOrDefaultAsync(x => x.Id == request.CommentId && x.PostId == request.PostId);

            if (comment == null)
            {
                return new Result<CommentDto>(new EntityNotFoundException(typeof(Comment), request.CommentId));
            }

            var authorizationResult = await _applicationAuthorizationService
             .AuthorizeAsync(comment, CommentOperationAuthorizationRequirement.Edit);

            if (authorizationResult.IsFailure)
            {
                return new Result<CommentDto>(authorizationResult.Exception!);
            }

            comment.Content = request.Content;

            await _commentRepository.UpdateAsync(comment);

            var commentView = await _commentMongoRepository.GetCommentViewById(comment.PostId,comment.Id);

            return await _commentResponseFactory.PrepareCommentDto(commentView!);

        }
    }
}

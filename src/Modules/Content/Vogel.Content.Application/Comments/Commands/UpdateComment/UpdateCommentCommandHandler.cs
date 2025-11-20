using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.Comments.Dtos;
using Vogel.Content.Application.Comments.Factories;
using Vogel.Content.Domain;
using Vogel.Content.Domain.Comments;
using Vogel.Content.MongoEntities.Comments;
namespace Vogel.Content.Application.Comments.Commands.UpdateComment
{
    public class UpdateCommentCommandHandler : IApplicationRequestHandler<UpdateCommentCommand, CommentDto>
    {
        private readonly IContentRepository<Comment> _commentRepository;
        private readonly ICommentResponseFactory _commentResponseFactory;
        private readonly CommentMongoRepository _commentMongoRepository;
        private readonly ISecurityContext _securityContext;

        public UpdateCommentCommandHandler(IContentRepository<Comment> commentRepository, ICommentResponseFactory commentResponseFactory, CommentMongoRepository commentMongoRepository, ISecurityContext securityContext)
        {
            _commentRepository = commentRepository;
            _commentResponseFactory = commentResponseFactory;
            _commentMongoRepository = commentMongoRepository;
            _securityContext = securityContext;
        }

        public async Task<Result<CommentDto>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            string userId = _securityContext.User!.Id;

            var comment = await _commentRepository.SingleOrDefaultAsync(x => x.Id == request.CommentId && x.PostId == request.PostId);

            if (comment == null)
            {
                return new Result<CommentDto>(new EntityNotFoundException(typeof(Comment), request.CommentId));
            }

            if (!comment.IsOwnedBy(userId))
            {
                return new Result<CommentDto>(new ForbiddenAccessException());
            }

            comment.Content = request.Content;

            await _commentRepository.UpdateAsync(comment);

            var commentView = await _commentMongoRepository.GetCommentViewById(comment.PostId,comment.Id);

            return await _commentResponseFactory.PrepareCommentDto(commentView!);

        }
    }
}

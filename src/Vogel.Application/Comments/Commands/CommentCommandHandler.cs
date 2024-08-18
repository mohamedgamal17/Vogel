using MediatR;
using MongoDB.Driver;
using Vogel.Application.Comments.Dtos;
using Vogel.Application.Comments.Factories;
using Vogel.Application.Comments.Polices;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Application.Security;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Repositories;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Domain.Comments;
using Vogel.Domain.Posts;
using Vogel.MongoDb.Entities.Comments;
namespace Vogel.Application.Comments.Commands
{
    public class CommentCommandHandler :
        IApplicationRequestHandler<CreateCommentCommand, CommentDto>,
        IApplicationRequestHandler<UpdateCommentCommand, CommentDto>,
        IApplicationRequestHandler<RemoveCommentCommand , Unit>
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Post> _postRepository;
        private readonly ISecurityContext _securityContext;
        private readonly ICommentResponseFactory _commentResponseFactory;
        private readonly CommentMongoRepository _commentMongoRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public CommentCommandHandler(IRepository<Comment> commentRepository, IRepository<Post> postRepository, ISecurityContext securityContext, ICommentResponseFactory commentResponseFactory, CommentMongoRepository commentMongoRepository, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _securityContext = securityContext;
            _commentResponseFactory = commentResponseFactory;
            _commentMongoRepository = commentMongoRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<CommentDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.FindByIdAsync(request.PostId);

            if(post == null)
            {
                return new Result<CommentDto>(new EntityNotFoundException(typeof(Post), request.PostId));
            }

            var comment = new Comment
            {
                Content = request.Content,
                PostId = post.Id,
                UserId = _securityContext.User!.Id,
                CommentId = request.CommentId
            };

            await _commentRepository.InsertAsync(comment);

            var commentView = await _commentMongoRepository.GetCommentViewById(comment.Id);

            return await _commentResponseFactory.PrepareCommentDto(commentView!);
        }

        public async Task<Result<CommentDto>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            var filter = new FilterDefinitionBuilder<Comment>()
                .And(
                    new FilterDefinitionBuilder<Comment>().Eq(x => x.PostId, request.PostId),
                    new FilterDefinitionBuilder<Comment>().Eq(x => x.Id, request.CommentId)
                );

            var comment = await _commentRepository.SingleOrDefaultAsync(x=> x.Id == request.CommentId && x.PostId == request.PostId);
            
            if(comment == null)
            {
                return new Result<CommentDto>(new EntityNotFoundException(typeof(Comment), request.CommentId));
            }

            var authorizationResult = await _applicationAuthorizationService
                .AuthorizeAsync(comment,CommentOperationAuthorizationRequirement.Edit);

            if (authorizationResult.IsFailure)
            {
                return new Result<CommentDto>(authorizationResult.Exception!);
            }

            comment.Content = request.Content;

            await _commentRepository.UpdateAsync(comment);

            var commentView = await _commentMongoRepository.GetCommentViewById(comment.Id);

            return await _commentResponseFactory.PrepareCommentDto(commentView!);
        }

        public async Task<Result<Unit>> Handle(RemoveCommentCommand request, CancellationToken cancellationToken)
        {

            var comment = await _commentRepository.SingleOrDefaultAsync(x=> x.Id == request.Id && x.PostId == request.PostId);

            if (comment == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(Comment), request.Id));
            }

            var authorizationResult = await _applicationAuthorizationService
                    .AuthorizeAsync(comment, CommentOperationAuthorizationRequirement.Delete);

            if (authorizationResult.IsFailure)
            {
                return authorizationResult;
            }

            await _commentRepository.DeleteAsync(comment);

            return Unit.Value;
        }
    }
}

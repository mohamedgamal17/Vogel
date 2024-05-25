using MediatR;
using MongoDB.Driver;
using Vogel.Application.Comments.Dtos;
using Vogel.Application.Comments.Factories;
using Vogel.Application.Comments.Polices;
using Vogel.Application.Common.Exceptions;
using Vogel.Application.Common.Interfaces;
using Vogel.Domain.Posts;
using Vogel.Domain.Utils;

namespace Vogel.Application.Comments.Commands
{
    public class CommentCommandHandler :
        IApplicationRequestHandler<CreateCommentCommand, CommentAggregateDto>,
        IApplicationRequestHandler<UpdateCommentCommand, CommentAggregateDto>,
        IApplicationRequestHandler<RemoveCommentCommand , Unit>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IMongoDbRepository<Post> _postRepository;
        private readonly ISecurityContext _securityContext;
        private readonly ICommentResponseFactory _commentResponseFactory;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public CommentCommandHandler(ICommentRepository commentRepository, IMongoDbRepository<Post> postRepository, ISecurityContext securityContext, ICommentResponseFactory commentResponseFactory, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _securityContext = securityContext;
            _commentResponseFactory = commentResponseFactory;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<CommentAggregateDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.FindByIdAsync(request.PostId);

            if(post == null)
            {
                return new Result<CommentAggregateDto>(new EntityNotFoundException(typeof(Post), request.PostId));
            }

            var comment = new Comment
            {
                Content = request.Content,
                PostId = post.Id,
                UserId = _securityContext.User!.Id
            };

            await _commentRepository.InsertAsync(comment);

            var commentAggregate = await _commentRepository.GetCommentAggregateView()
                .Match(x => x.Id == comment.Id)
                .SingleAsync();

            return await _commentResponseFactory.PrepareCommentAggregateDto(commentAggregate);
        }

        public async Task<Result<CommentAggregateDto>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            var filter = new FilterDefinitionBuilder<Comment>()
                .And(
                    new FilterDefinitionBuilder<Comment>().Eq(x => x.PostId, request.PostId),
                    new FilterDefinitionBuilder<Comment>().Eq(x => x.Id, request.Id)
                );

            var comment = await _commentRepository.FindAsync(filter);
            
            if(comment == null)
            {
                return new Result<CommentAggregateDto>(new EntityNotFoundException(typeof(Comment), request.Id));
            }

            var authorizationResult = await _applicationAuthorizationService
                .AuthorizeAsync(comment,CommentOperationAuthorizationRequirement.Edit);

            if (authorizationResult.IsFailure)
            {
                return new Result<CommentAggregateDto>(authorizationResult.Exception!);
            }

            comment.Content = request.Content;

            await _commentRepository.UpdateAsync(comment);

            var commentAggregate = await _commentRepository.GetCommentAggregateView()
                .Match(x => x.Id == comment.Id)
                .SingleAsync();

            return await _commentResponseFactory.PrepareCommentAggregateDto(commentAggregate);
        }

        public async Task<Result<Unit>> Handle(RemoveCommentCommand request, CancellationToken cancellationToken)
        {
            var filter = new FilterDefinitionBuilder<Comment>()
               .And(
                   new FilterDefinitionBuilder<Comment>().Eq(x => x.PostId, request.PostId),
                   new FilterDefinitionBuilder<Comment>().Eq(x => x.Id, request.Id)
               );

            var comment = await _commentRepository.FindAsync(filter);

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

        private CommentDto PrepareCommentDto(Comment comment)
        {
            var dto = new CommentDto
            {
                Id = comment.Id,
                PostId = comment.PostId,
                Content = comment.Content,
                UserId = comment.UserId
            };

            return dto;
        }
    }
}

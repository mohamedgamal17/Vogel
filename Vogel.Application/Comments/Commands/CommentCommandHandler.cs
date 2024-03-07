using MediatR;
using MongoDB.Driver;
using Vogel.Application.Comments.Dtos;
using Vogel.Application.Common.Exceptions;
using Vogel.Application.Common.Interfaces;
using Vogel.Domain;
using Vogel.Domain.Utils;

namespace Vogel.Application.Comments.Commands
{
    public class CommentCommandHandler :
        IApplicationRequestHandler<CreateCommentCommand, CommentDto>,
        IApplicationRequestHandler<UpdateCommentCommand, CommentDto>,
        IApplicationRequestHandler<RemoveCommentCommand , Unit>
    {
        private readonly IMongoDbRepository<Comment> _commentRepository;
        private readonly IMongoDbRepository<Post> _postRepository;
        private readonly ISecurityContext _securityContext;

        public CommentCommandHandler(IMongoDbRepository<Comment> commentRepository, IMongoDbRepository<Post> postRepository, ISecurityContext securityContext)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _securityContext = securityContext;
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
                UserId = _securityContext.User!.Id
            };


            await _commentRepository.InsertAsync(comment);

            return PrepareCommentDto(comment);
        }

        public async Task<Result<CommentDto>> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            var filter = new FilterDefinitionBuilder<Comment>()
                .And(
                    new FilterDefinitionBuilder<Comment>().Eq(x => x.PostId, request.PostId),
                    new FilterDefinitionBuilder<Comment>().Eq(x => x.Id, request.Id)
                );

            var comment = await _commentRepository.FindAsync(filter);
            
            if(comment == null)
            {
                return new Result<CommentDto>(new EntityNotFoundException(typeof(Comment), request.Id));
            }

            comment.Content = request.Content;

            await _commentRepository.UpdateAsync(comment);

            return PrepareCommentDto(comment);
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

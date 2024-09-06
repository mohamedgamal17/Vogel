using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.Comments.Dtos;
using Vogel.Content.Application.Comments.Factories;
using Vogel.Content.Domain.Comments;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.Comments;
using Vogel.Social.Domain;

namespace Vogel.Content.Application.Comments.Commands.CreateComment
{
    public class CreateCommentCommandHandler : IApplicationRequestHandler<CreateCommentCommand, CommentDto>
    {
        private readonly ISocialRepository<Comment> _commentRepository;
        private readonly ISocialRepository<Post> _postRepository;
        private readonly ISecurityContext _securityContext;
        private readonly ICommentResponseFactory _commentResponseFactory;
        private readonly CommentMongoRepository _commentMongoRepository;

        public CreateCommentCommandHandler(ISocialRepository<Comment> commentRepository, ISocialRepository<Post> postRepository, ISecurityContext securityContext, ICommentResponseFactory commentResponseFactory, CommentMongoRepository commentMongoRepository)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _securityContext = securityContext;
            _commentResponseFactory = commentResponseFactory;
            _commentMongoRepository = commentMongoRepository;
        }

        public async Task<Result<CommentDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.FindByIdAsync(request.PostId);

            if (post == null)
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

            var commentView = await _commentMongoRepository.GetCommentViewById(comment.PostId,comment.Id);

            return await _commentResponseFactory.PrepareCommentDto(commentView!);
        }
    }
}

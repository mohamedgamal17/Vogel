using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.Comments.Dtos;
using Vogel.Content.Application.Comments.Factories;
using Vogel.Content.Domain.Comments;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.Comments;
using Vogel.Content.MongoEntities.Posts;
namespace Vogel.Content.Application.Comments.Queries.GetCommentById
{
    public class GetCommentByIdQueryHandler : IApplicationRequestHandler<GetCommentByIdQuery, CommentDto>
    {
        private readonly CommentMongoRepository _commentMongoRepository;
        private readonly PostMongoRepository _postMongoRepository;
        private readonly ICommentResponseFactory _commentResponseFactory;
        public GetCommentByIdQueryHandler(CommentMongoRepository commentMongoRepository, PostMongoRepository postMongoRepository, ICommentResponseFactory commentResponseFactory)
        {
            _commentMongoRepository = commentMongoRepository;
            _postMongoRepository = postMongoRepository;
            _commentResponseFactory = commentResponseFactory;
        }

        public async Task<Result<CommentDto>> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
        {
            var post = await _postMongoRepository.FindByIdAsync(request.PostId);

            if(post == null)
            {
                return new Result<CommentDto>(new EntityNotFoundException(typeof(Post), request.PostId));
            }

            var comment = await _commentMongoRepository.GetCommentViewById(request.PostId, request.CommentId);

            if(comment == null)
            {
                return new Result<CommentDto>(new EntityNotFoundException(typeof(Comment), request.CommentId));
            }

            return await _commentResponseFactory.PrepareCommentDto(comment);
        }
    }
}

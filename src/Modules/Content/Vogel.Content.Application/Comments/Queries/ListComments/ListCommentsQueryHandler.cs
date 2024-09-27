using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Content.Application.Comments.Dtos;
using Vogel.Content.Application.Comments.Factories;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.Comments;
using Vogel.Content.MongoEntities.Posts;
namespace Vogel.Content.Application.Comments.Queries.ListComments
{
    public class ListCommentsQueryHandler : IApplicationRequestHandler<ListCommentsQuery, Paging<CommentDto>>
    {
        private readonly PostMongoRepository _postMongoRepository;
        private readonly CommentMongoRepository _commentMongoRepository;
        private readonly ICommentResponseFactory _commentResponseFactory;

        public ListCommentsQueryHandler(PostMongoRepository postMongoRepository, CommentMongoRepository commentMongoRepository, ICommentResponseFactory commentResponseFactory)
        {
            _postMongoRepository = postMongoRepository;
            _commentMongoRepository = commentMongoRepository;
            _commentResponseFactory = commentResponseFactory;
        }

        public async Task<Result<Paging<CommentDto>>> Handle(ListCommentsQuery request, CancellationToken cancellationToken)
        {
            var post = await _postMongoRepository.FindByIdAsync(request.PostId);

            if (post == null)
            {
                return new Result<Paging<CommentDto>>(new EntityNotFoundException(typeof(Post), request.PostId));
            }

            var paged = await _commentMongoRepository.ListCommentView(request.PostId, request.Cursor, request.Limit, request.Asending);

            var response = new Paging<CommentDto>
            {
                Data = await _commentResponseFactory.PreapreListCommentDto(paged.Data),
                Info = paged.Info
            };

            return response;
        }
    }
}

using Vogel.Application.Comments.Dtos;
using Vogel.Application.Comments.Factories;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Results;
using Vogel.Domain.Comments;
using Vogel.MongoDb.Entities.Comments;
using Vogel.MongoDb.Entities.Common;
namespace Vogel.Application.Comments.Queries
{
    public class CommentQueryHandler : 
        IApplicationRequestHandler<ListCommentsQuery, Paging<CommentDto>>,
        IApplicationRequestHandler<GetCommentQuery, CommentDto>,
        IApplicationRequestHandler<GetSubCommentsQuery, Paging<CommentDto>>
    {
        private readonly CommentMongoRepository _commentMongoRepository;

        private readonly ICommentResponseFactory _commentResponseFactory;

        public CommentQueryHandler(CommentMongoRepository commentMongoRepository, ICommentResponseFactory commentResponseFactory)
        {
            _commentMongoRepository = commentMongoRepository;
            _commentResponseFactory = commentResponseFactory;
        }

        public async Task<Result<Paging<CommentDto>>> Handle(ListCommentsQuery request, CancellationToken cancellationToken)
        {
            var paged = await _commentMongoRepository.GetCommentViewPaged(request.PostId, null, request.Cursor, request.Limit,
                request.Asending);

            var result = new Paging<CommentDto>
            {
                Data = await _commentResponseFactory.PreapreListCommentDto(paged.Data),
                Info = paged.Info
            };

            return result;
        }

        public async Task<Result<CommentDto>> Handle(GetCommentQuery request, CancellationToken cancellationToken)
        {
            var comment = await _commentMongoRepository.GetCommentViewById(request.CommentId);

            if(comment == null)
            {
                return new Result<CommentDto>(new EntityNotFoundException(typeof(Comment), request.CommentId));
            }
            return await _commentResponseFactory.PrepareCommentDto(comment);
        }


        public async Task<Result<Paging<CommentDto>>> Handle(GetSubCommentsQuery request, CancellationToken cancellationToken)
        {
            var paged = await _commentMongoRepository.GetCommentViewPaged(request.PostId, request.CommentId, request.Cursor, request.Limit, request.Asending);


            var result = new Paging<CommentDto>()
            {
                Data = await _commentResponseFactory.PreapreListCommentDto(paged.Data),
                Info = paged.Info
            };

            return result;
        }
    }
}

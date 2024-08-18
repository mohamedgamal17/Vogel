using Vogel.Application.CommentReactions.Dtos;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.MongoDb.Entities.CommentReactions;
using Vogel.Application.CommentReactions.Factories;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Domain.Comments;
using Vogel.MongoDb.Entities.Common;
using Vogel.BuildingBlocks.Shared.Results;
namespace Vogel.Application.CommentReactions.Queries
{
    public class CommentReactionQueryHandler :
        IApplicationRequestHandler<ListCommentReactionQuery, Paging<CommentReactionDto>>,
        IApplicationRequestHandler<GetCommentReactionQuery , CommentReactionDto>
    {
        private readonly CommentReactionMongoRepository _commentReactionMongoRepository;

        private readonly ICommentReactionResponseFactory _CommentReactionResponseFactory;

        public CommentReactionQueryHandler(CommentReactionMongoRepository commentReactionMongoRepository, ICommentReactionResponseFactory commentReactionResponseFactory)
        {
            _commentReactionMongoRepository = commentReactionMongoRepository;
            _CommentReactionResponseFactory = commentReactionResponseFactory;
        }

        public async Task<Result<Paging<CommentReactionDto>>> Handle(ListCommentReactionQuery request, CancellationToken cancellationToken)
        {
            var paged = await _commentReactionMongoRepository.GetReactionViewPaged(request.CommentId, request.Cursor, request.Limit, request.Asending);


            var result = new Paging<CommentReactionDto>
            {
                Data = await _CommentReactionResponseFactory.PrepareListCommentReactionDto(paged.Data),
                Info = paged.Info
            };

            return result;
        }

        public async Task<Result<CommentReactionDto>> Handle(GetCommentReactionQuery request, CancellationToken cancellationToken)
        {
            var data = await _commentReactionMongoRepository.GetReactionViewById(request.CommentId, request.Id);

            if(data == null)
            {
                return new Result<CommentReactionDto>(new EntityNotFoundException(typeof(CommentReaction), request.Id));
            }

            return await _CommentReactionResponseFactory.PrepareCommentReactionDto(data);
        }

    }
}

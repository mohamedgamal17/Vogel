using Vogel.Application.CommentReactions.Dtos;
using Vogel.Application.Common.Models;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Results;
using Vogel.MongoDb.Entities.CommentReactions;
using MongoDB.Driver;
using Vogel.Application.CommentReactions.Factories;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.Domain.Comments;
namespace Vogel.Application.CommentReactions.Queries
{
    public class CommentReactionQueryHandler :
        IApplicationRequestHandler<ListCommentReactionQuery, Paging<CommentReactionDto>>,
        IApplicationRequestHandler<GetCommentReactionQuery , CommentReactionDto>
    {
        private readonly CommentReactionMongoViewRepository _commentReactionMongoViewRepository;

        private readonly ICommentReactionResponseFactory _CommentReactionResponseFactory;
        public CommentReactionQueryHandler(CommentReactionMongoViewRepository commentReactionMongoViewRepository, ICommentReactionResponseFactory commentReactionResponseFactory)
        {
            _commentReactionMongoViewRepository = commentReactionMongoViewRepository;
            _CommentReactionResponseFactory = commentReactionResponseFactory;
        }

        public async Task<Result<Paging<CommentReactionDto>>> Handle(ListCommentReactionQuery request, CancellationToken cancellationToken)
        {
            var query = _commentReactionMongoViewRepository.AsMongoCollection().Aggregate()
                .Match(x => x.CommentId == request.CommentId);

            query = SortQuery(query, request);

            var data = await Paginate(query, request);

            var prepareResponseTask = _CommentReactionResponseFactory.PrepareListCommentReactionDto(data);
            var preaprePaginagInfoTask = PreparePagingInfo(query, request);

            await Task.WhenAll(prepareResponseTask, preaprePaginagInfoTask);

            var paged = new Paging<CommentReactionDto>
            {
                Data = prepareResponseTask.Result,
                Info = preaprePaginagInfoTask.Result
            };

            return paged;
        }

        public async Task<Result<CommentReactionDto>> Handle(GetCommentReactionQuery request, CancellationToken cancellationToken)
        {
            var query = _commentReactionMongoViewRepository.AsMongoCollection().Aggregate()
                    .Match(x => x.CommentId == request.CommentId)
                    .Match(x => x.Id == request.Id);

            var data = await query.SingleOrDefaultAsync();

            if(data == null)
            {
                return new Result<CommentReactionDto>(new EntityNotFoundException(typeof(CommentReaction), request.Id));
            }

            return await _CommentReactionResponseFactory.PrepareCommentReactionDto(data);
        }

        private IAggregateFluent<CommentReactionMongoView> SortQuery(IAggregateFluent<CommentReactionMongoView> query, PagingParams request)
        {
            return request.Asending ? query.SortBy(x => x.Id) : query.SortByDescending(x => x.Id);
        }

        private async Task<List<CommentReactionMongoView>> Paginate(IAggregateFluent<CommentReactionMongoView> query, PagingParams request)
        {
            if (request.Cursor != null)
            {
                var filter = request.Asending ? Builders<CommentReactionMongoView>.Filter.Gte(x => x.Id, request.Cursor)
                    : Builders<CommentReactionMongoView>.Filter.Lte(x => x.Id, request.Cursor);

                query = query.Match(filter);
            }

            return await query.Limit(request.Limit).ToListAsync();
        }

        private async Task<PagingInfo> PreparePagingInfo(IAggregateFluent<CommentReactionMongoView> query, PagingParams request)
        {
            if (request.Cursor != null)
            {
                var previosFilter = request.Asending ? Builders<CommentReactionMongoView>.Filter.Lt(x => x.Id, request.Cursor)
                : Builders<CommentReactionMongoView>.Filter.Gt(x => x.Id, request.Cursor);

                var nextFilter = request.Asending ? Builders<CommentReactionMongoView>.Filter.Gt(x => x.Id, request.Cursor)
                    : Builders<CommentReactionMongoView>.Filter.Lt(x => x.Id, request.Cursor);

                var next = await query.Match(nextFilter).Skip(request.Limit - 1).FirstOrDefaultAsync();

                var previos = await query.Match(previosFilter).FirstOrDefaultAsync();

                return new PagingInfo(next?.Id, previos?.Id, request.Asending);
            }
            else
            {
                var next = await query.Skip(request.Limit - 1).FirstOrDefaultAsync();

                return new PagingInfo(next?.Id, null, request.Asending);
            }
        }

     
    }
}

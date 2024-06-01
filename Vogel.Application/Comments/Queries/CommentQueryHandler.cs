using MongoDB.Driver;
using Vogel.Application.Comments.Dtos;
using Vogel.Application.Comments.Factories;
using Vogel.Application.Common.Models;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Results;
using Vogel.Domain;
using Vogel.Domain.Posts;
using Vogel.MongoDb.Entities.Comments;
namespace Vogel.Application.Comments.Queries
{
    public class CommentQueryHandler : 
        IApplicationRequestHandler<ListCommentsQuery, Paging<CommentAggregateDto>>,
        IApplicationRequestHandler<GetCommentQuery, CommentAggregateDto>
    {
        private readonly CommentMongoViewRepository _commentMongoViewRepository;

        private readonly ICommentResponseFactory _commentResponseFactory;

        public CommentQueryHandler(CommentMongoViewRepository commentMongoViewRepository, ICommentResponseFactory commentResponseFactory)
        {
            _commentMongoViewRepository = commentMongoViewRepository;
            _commentResponseFactory = commentResponseFactory;
        }

        public async Task<Result<Paging<CommentAggregateDto>>> Handle(ListCommentsQuery request, CancellationToken cancellationToken)
        {
            var query = _commentMongoViewRepository.AsMongoCollection()
                .Aggregate()
                .Match(x => x.PostId == request.PostId);

            var sortedQuery = SortQuery(query, request);

            var data = await Paginate(sortedQuery, request);

            var paginInfo = await PreparePagingInfo(query, request);

            var response = await _commentResponseFactory.PreapreListCommentAggregateDto(data);

            var paged = new Paging<CommentAggregateDto>()
            {
                Data = response,
                Info = paginInfo
            };

            return paged;
        }

        public async Task<Result<CommentAggregateDto>> Handle(GetCommentQuery request, CancellationToken cancellationToken)
        {

            var query = _commentMongoViewRepository.AsMongoCollection()
                .Aggregate()
                .Match(x => x.PostId == request.PostId && x.Id == request.CommentId);

            var comment = await query.SingleOrDefaultAsync();

            if(comment == null)
            {
                return new Result<CommentAggregateDto>(new EntityNotFoundException(typeof(Comment), request.CommentId));
            }


            return await _commentResponseFactory.PrepareCommentAggregateDto(comment);
        }


        private IAggregateFluent<CommentMongoView> SortQuery(IAggregateFluent<CommentMongoView> query, ListCommentsQuery request)
        {
            return request.Asending ? query.SortBy(x => x.Id) : query.SortByDescending(x => x.Id);
        }

        private async Task<List<CommentMongoView>> Paginate(IAggregateFluent<CommentMongoView> query, ListCommentsQuery request)
        {
            if (request.Cursor != null)
            {
                var filter = request.Asending ? Builders<CommentMongoView>.Filter.Gte(x => x.Id, request.Cursor)
                    : Builders<CommentMongoView>.Filter.Lte(x => x.Id, request.Cursor);

                query = query.Match(filter);
            }

            return await query.Limit(request.Limit).ToListAsync();
        }

        private async Task<PagingInfo> PreparePagingInfo(IAggregateFluent<CommentMongoView> query, ListCommentsQuery request)
        {
            if (request.Cursor != null)
            {
                var previosFilter = request.Asending ? Builders<CommentMongoView>.Filter.Lt(x => x.Id, request.Cursor)
                : Builders<CommentMongoView>.Filter.Gt(x => x.Id, request.Cursor);

                var nextFilter = request.Asending ? Builders<CommentMongoView>.Filter.Gt(x => x.Id, request.Cursor)
                    : Builders<CommentMongoView>.Filter.Lt(x => x.Id, request.Cursor);

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

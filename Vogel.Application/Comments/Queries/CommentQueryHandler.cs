using MongoDB.Driver;
using Vogel.Application.Comments.Dtos;
using Vogel.Application.Comments.Factories;
using Vogel.Application.Common.Exceptions;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Common.Models;
using Vogel.Domain;
using Vogel.Domain.Utils;

namespace Vogel.Application.Comments.Queries
{
    public class CommentQueryHandler : 
        IApplicationRequestHandler<ListCommentsQuery, Paging<CommentAggregateDto>>,
        IApplicationRequestHandler<GetCommentQuery, CommentAggregateDto>
    {
        private readonly ICommentRepository _commentRepository;

        private readonly ICommentResponseFactory _commentResponseFactory;
        public CommentQueryHandler(ICommentRepository commentRepository, ICommentResponseFactory commentResponseFactory)
        {
            _commentRepository = commentRepository;
            _commentResponseFactory = commentResponseFactory;
        }
        public async Task<Result<Paging<CommentAggregateDto>>> Handle(ListCommentsQuery request, CancellationToken cancellationToken)
        {
            var query = _commentRepository.GetCommentAggregateView()
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

            var query = _commentRepository.GetCommentAggregateView()
                .Match(x => x.PostId == request.PostId && x.Id == request.CommentId);

            var comment = await query.SingleOrDefaultAsync();

            if(comment == null)
            {
                return new Result<CommentAggregateDto>(new EntityNotFoundException(typeof(Comment), request.CommentId));
            }


            return await _commentResponseFactory.PrepareCommentAggregateDto(comment);
        }


        private IAggregateFluent<CommentAggregateView> SortQuery(IAggregateFluent<CommentAggregateView> query, ListCommentsQuery request)
        {
            return request.Asending ? query.SortBy(x => x.Id) : query.SortByDescending(x => x.Id);
        }

        private async Task<List<CommentAggregateView>> Paginate(IAggregateFluent<CommentAggregateView> query, ListCommentsQuery request)
        {
            if (request.Cursor != null)
            {
                var filter = request.Asending ? Builders<CommentAggregateView>.Filter.Gte(x => x.Id, request.Cursor)
                    : Builders<CommentAggregateView>.Filter.Lte(x => x.Id, request.Cursor);

                query = query.Match(filter);
            }

            return await query.Limit(request.Limit).ToListAsync();
        }

        private async Task<PagingInfo> PreparePagingInfo(IAggregateFluent<CommentAggregateView> query, ListCommentsQuery request)
        {
            if (request.Cursor != null)
            {
                var previosFilter = request.Asending ? Builders<CommentAggregateView>.Filter.Lt(x => x.Id, request.Cursor)
                : Builders<CommentAggregateView>.Filter.Gt(x => x.Id, request.Cursor);

                var nextFilter = request.Asending ? Builders<CommentAggregateView>.Filter.Gt(x => x.Id, request.Cursor)
                    : Builders<CommentAggregateView>.Filter.Lt(x => x.Id, request.Cursor);

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

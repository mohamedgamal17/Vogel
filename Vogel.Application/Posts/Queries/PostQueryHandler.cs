using MongoDB.Driver;
using Vogel.Application.Common.Models;
using Vogel.Application.Posts.Dtos;
using Vogel.Application.Posts.Factories;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Results;
using Vogel.Domain.Posts;
using Vogel.MongoDb.Entities.Posts;
namespace Vogel.Application.Posts.Queries
{
    public class PostQueryHandler :
        IApplicationRequestHandler<ListPostQuery, Paging<PostAggregateDto>>,
        IApplicationRequestHandler<ListUserPostQuery, Paging<PostAggregateDto>>,
        IApplicationRequestHandler<GetUserPostById, PostAggregateDto>,
        IApplicationRequestHandler<GetPostByIdQuery, PostAggregateDto>
    {
        private readonly PostMongoViewRepository _postMongoViewRepository;
        private readonly IPostResponseFactory _postResponseFactory;

        public PostQueryHandler(PostMongoViewRepository postMongoViewRepository, IPostResponseFactory postResponseFactory)
        {
            _postMongoViewRepository = postMongoViewRepository;
            _postResponseFactory = postResponseFactory;
        }

        public async Task<Result<Paging<PostAggregateDto>>> Handle(ListPostQuery request, CancellationToken cancellationToken)
        {
            var query = _postMongoViewRepository.AsMongoCollection().Aggregate();

            query = SortQuery(query, request);

            var data = await Paginate(query, request);

            var prepareResponseTask = _postResponseFactory.PrepareListPostAggregateDto(data);
            var preaprePaginagInfoTask = PreparePagingInfo(query, request);

            await Task.WhenAll(prepareResponseTask, preaprePaginagInfoTask);

            var paged = new Paging<PostAggregateDto>
            {
                Data = prepareResponseTask.Result,
                Info = preaprePaginagInfoTask.Result
            };

            return paged;
        }

        private IAggregateFluent<PostMongoView> SortQuery(IAggregateFluent<PostMongoView> query, ListPostQueryBase request)
        {
            return request.Asending ? query.SortBy(x => x.Id) : query.SortByDescending(x => x.Id);
        }

        public async Task<Result<PostAggregateDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _postMongoViewRepository.FindByIdAsync(request.Id);

            if (result == null)
            {
                return new Result<PostAggregateDto>(new EntityNotFoundException(typeof(Post), request.Id));
            }


            return await _postResponseFactory.PreparePostAggregateDto(result);
        }

        public async Task<Result<Paging<PostAggregateDto>>> Handle(ListUserPostQuery request, CancellationToken cancellationToken)
        {
            var query = _postMongoViewRepository.AsMongoCollection()
                .Aggregate()
                .Match(x => x.UserId == request.UserId);
 
            query = SortQuery(query, request);

            var data = await Paginate(query, request);

            var prepareResponseTask = _postResponseFactory.PrepareListPostAggregateDto(data);
            var preaprePaginagInfoTask = PreparePagingInfo(query, request);

            await Task.WhenAll(prepareResponseTask, preaprePaginagInfoTask);

            var paged = new Paging<PostAggregateDto>
            {
                Data = prepareResponseTask.Result,
                Info = preaprePaginagInfoTask.Result
            };

            return paged;
        }

        public async Task<Result<PostAggregateDto>> Handle(GetUserPostById request, CancellationToken cancellationToken)
        {
              var result = await _postMongoViewRepository.AsMongoCollection()
                  .Aggregate()
                  .Match(x => x.UserId == request.UserId)
                  .Match(x => x.Id == request.Id && x.UserId == request.UserId)
                  .SingleOrDefaultAsync();

            if (result == null)
            {
                return new Result<PostAggregateDto>(new EntityNotFoundException(typeof(Post), request.Id));
            }

            return await _postResponseFactory.PreparePostAggregateDto(result);
        }
        private async Task<List<PostMongoView>> Paginate(IAggregateFluent<PostMongoView> query, ListPostQueryBase request)
        {
            if (request.Cursor != null)
            {
                var filter = request.Asending ? Builders<PostMongoView>.Filter.Gte(x => x.Id, request.Cursor)
                    : Builders<PostMongoView>.Filter.Lte(x => x.Id, request.Cursor);

                query = query.Match(filter);
            }

            return await query.Limit(request.Limit).ToListAsync();
        }

        private async Task<PagingInfo> PreparePagingInfo(IAggregateFluent<PostMongoView> query, ListPostQueryBase request)
        {
            if (request.Cursor != null)
            {
                var previosFilter = request.Asending ? Builders<PostMongoView>.Filter.Lt(x => x.Id, request.Cursor)
                : Builders<PostMongoView>.Filter.Gt(x => x.Id, request.Cursor);

                var nextFilter = request.Asending ? Builders<PostMongoView>.Filter.Gt(x => x.Id, request.Cursor)
                    : Builders<PostMongoView>.Filter.Lt(x => x.Id, request.Cursor);

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

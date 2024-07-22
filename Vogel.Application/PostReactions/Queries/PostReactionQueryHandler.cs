using MongoDB.Driver;
using Vogel.Application.Common.Models;
using Vogel.Application.PostReactions.Dtos;
using Vogel.Application.PostReactions.Factories;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Results;
using Vogel.Domain.Posts;
using Vogel.MongoDb.Entities.PostReactions;
using Vogel.MongoDb.Entities.Posts;
namespace Vogel.Application.PostReactions.Queries
{
    public class PostReactionQueryHandler :
        IApplicationRequestHandler<ListPostReactionQuery, Paging<PostReactionDto>>,
        IApplicationRequestHandler<GetPostReactionQuery, PostReactionDto>
    {
        private readonly PostReactionMongoViewRepository _postReactionMongoViewRepository;

        private readonly IPostReactionResponseFactory _postReactionResponseFactory;

        public PostReactionQueryHandler(PostReactionMongoViewRepository postReactionMongoViewRepository, IPostReactionResponseFactory postReactionResponseFactory)
        {
            _postReactionMongoViewRepository = postReactionMongoViewRepository;
            _postReactionResponseFactory = postReactionResponseFactory;
        }

        public async Task<Result<Paging<PostReactionDto>>> Handle(ListPostReactionQuery request, CancellationToken cancellationToken)
        {
            var query = _postReactionMongoViewRepository.AsMongoCollection().Aggregate();

            query = SortQuery(query, request);

            var data = await Paginate(query, request);

            var prepareResponseTask = _postReactionResponseFactory.PreparePostReactionDto(data);

            var preparePaginagInfoTask = PreparePagingInfo(query, request);

            await Task.WhenAll(prepareResponseTask, preparePaginagInfoTask);

            var paged = new Paging<PostReactionDto>
            {
                Data = prepareResponseTask.Result,
                Info = preparePaginagInfoTask.Result
            };

            return paged;
        }

        public async Task<Result<PostReactionDto>> Handle(GetPostReactionQuery request, CancellationToken cancellationToken)
        {
            var reaction = await _postReactionMongoViewRepository.AsMongoCollection().Aggregate()
                    .Match(x => x.PostId == request.PostId)
                    .Match(x => x.Id == request.Id)
                    .SingleOrDefaultAsync();

            if(reaction == null)
            {
                return new Result<PostReactionDto>(new EntityNotFoundException(typeof(PostReaction), request.Id));
            }


            return await _postReactionResponseFactory.PreparePostReactionDto(reaction);

        }

        private async Task<List<PostReactionMongoView>> Paginate(IAggregateFluent<PostReactionMongoView> query, PagingParams request)
        {
            if (request.Cursor != null)
            {
                var filter = request.Asending ? Builders<PostReactionMongoView>.Filter.Gte(x => x.Id, request.Cursor)
                    : Builders<PostReactionMongoView>.Filter.Lte(x => x.Id, request.Cursor);

                query = query.Match(filter);
            }

            return await query.Limit(request.Limit).ToListAsync();
        }
        private async Task<PagingInfo> PreparePagingInfo(IAggregateFluent<PostReactionMongoView> query, PagingParams request)
        {
            if (request.Cursor != null)
            {
                var previosFilter = request.Asending ? Builders<PostReactionMongoView>.Filter.Lt(x => x.Id, request.Cursor)
                : Builders<PostReactionMongoView>.Filter.Gt(x => x.Id, request.Cursor);

                var nextFilter = request.Asending ? Builders<PostReactionMongoView>.Filter.Gt(x => x.Id, request.Cursor)
                    : Builders<PostReactionMongoView>.Filter.Lt(x => x.Id, request.Cursor);

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

        private IAggregateFluent<PostReactionMongoView> SortQuery(IAggregateFluent<PostReactionMongoView> query, PagingParams request)
        {
            return request.Asending ? query.SortBy(x => x.Id) : query.SortByDescending(x => x.Id);
        }

    }
}

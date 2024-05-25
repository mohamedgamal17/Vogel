using MongoDB.Driver;
using Vogel.Application.Common.Exceptions;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Common.Models;
using Vogel.Application.Posts.Dtos;
using Vogel.Application.Posts.Factories;
using Vogel.Domain.Medias;
using Vogel.Domain.Posts;
using Vogel.Domain.Utils;

namespace Vogel.Application.Posts.Queries
{
    public class PostQueryHandler :
        IApplicationRequestHandler<ListPostQuery, Paging<PostAggregateDto>>,
        IApplicationRequestHandler<ListUserPostQuery, Paging<PostAggregateDto>>,
        IApplicationRequestHandler<GetUserPostById, PostAggregateDto>,
        IApplicationRequestHandler<GetPostByIdQuery, PostAggregateDto>
    {
        private readonly IPostRepository _postRepository;
        private readonly IMongoDbRepository<Media> _mediaRepository;
        private readonly IMongoDbRepository<User> _userRepository;
        private readonly IPostResponseFactory _postResponseFactory;

        public PostQueryHandler(IPostRepository postRepository, IMongoDbRepository<Media> mediaRepository, IMongoDbRepository<User> userRepository, IPostResponseFactory postResponseFactory)
        {
            _postRepository = postRepository;
            _mediaRepository = mediaRepository;
            _userRepository = userRepository;
            _postResponseFactory = postResponseFactory;
        }

        public async Task<Result<Paging<PostAggregateDto>>> Handle(ListPostQuery request, CancellationToken cancellationToken)
        {
            var query = _postRepository.GetPostAggregateView();

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

 

        private IAggregateFluent<PostAggregateView> SortQuery(IAggregateFluent<PostAggregateView> query, ListPostQueryBase request)
        {
            return request.Asending ? query.SortBy(x => x.Id) : query.SortByDescending(x => x.Id);
        }

        public async Task<Result<PostAggregateDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _postRepository.GetPostAggregateView()
              .Match(x => x.Id == request.Id)
              .SingleOrDefaultAsync();

            if (result == null)
            {
                return new Result<PostAggregateDto>(new EntityNotFoundException(typeof(Post), request.Id));
            }


            return await _postResponseFactory.PreparePostAggregateDto(result);
        }

        public async Task<Result<Paging<PostAggregateDto>>> Handle(ListUserPostQuery request, CancellationToken cancellationToken)
        {
            var mediaCollection = _mediaRepository.AsMongoCollection();

            var userCollection = _userRepository.AsMongoCollection();

            var query = _postRepository.GetPostAggregateView()
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
            var mediaCollection = _mediaRepository.AsMongoCollection();

            var userCollection = _userRepository.AsMongoCollection();

            var result = await _postRepository.GetPostAggregateView()
              .Match(x => x.Id == request.Id && x.UserId == request.UserId)
              .SingleOrDefaultAsync();

            if (result == null)
            {
                return new Result<PostAggregateDto>(new EntityNotFoundException(typeof(Post), request.Id));
            }

            return await _postResponseFactory.PreparePostAggregateDto(result);
        }
        private async Task<List<PostAggregateView>> Paginate(IAggregateFluent<PostAggregateView> query, ListPostQueryBase request)
        {
            if (request.Cursor != null)
            {
                var filter = request.Asending ? Builders<PostAggregateView>.Filter.Gte(x => x.Id, request.Cursor)
                    : Builders<PostAggregateView>.Filter.Lte(x => x.Id, request.Cursor);

                query = query.Match(filter);
            }

            return await query.Limit(request.Limit).ToListAsync();
        }

        private async Task<PagingInfo> PreparePagingInfo(IAggregateFluent<PostAggregateView> query, ListPostQueryBase request)
        {
            if (request.Cursor != null)
            {
                var previosFilter = request.Asending ? Builders<PostAggregateView>.Filter.Lt(x => x.Id, request.Cursor)
                : Builders<PostAggregateView>.Filter.Gt(x => x.Id, request.Cursor);

                var nextFilter = request.Asending ? Builders<PostAggregateView>.Filter.Gt(x => x.Id, request.Cursor)
                    : Builders<PostAggregateView>.Filter.Lt(x => x.Id, request.Cursor);

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

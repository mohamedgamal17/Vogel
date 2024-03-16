using MongoDB.Driver;
using Vogel.Application.Common.Exceptions;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Common.Models;
using Vogel.Application.Posts.Dtos;
using Vogel.Application.Posts.Factories;
using Vogel.Domain;
using Vogel.Domain.Utils;

namespace Vogel.Application.Posts.Queries
{
    public class PostQueryHandler :
        IApplicationRequestHandler<ListPostPostQuery, Paging<PostAggregateDto>>,
        IApplicationRequestHandler<GetPostByIdQuery, PostAggregateDto>
    {
        private readonly IMongoDbRepository<Post> _postRepository;
        private readonly IMongoDbRepository<Media> _mediaRepository;
        private readonly IMongoDbRepository<User> _userRepository;
        private readonly IPostResponseFactory _postResponseFactory;

        public PostQueryHandler(IMongoDbRepository<Post> postRepository, IMongoDbRepository<Media> mediaRepository, IMongoDbRepository<User> userRepository, IPostResponseFactory postResponseFactory)
        {
            _postRepository = postRepository;
            _mediaRepository = mediaRepository;
            _userRepository = userRepository;
            _postResponseFactory = postResponseFactory;
        }

        public async Task<Result<Paging<PostAggregateDto>>> Handle(ListPostPostQuery request, CancellationToken cancellationToken)
        {
            var mediaCollection = _mediaRepository.AsMongoCollection();

            var userCollection = _userRepository.AsMongoCollection();

            var query = _postRepository.AsMongoCollection().Aggregate()
                .Lookup<Post, Media, PostAggregate>(mediaCollection,
                    x => x.MediaId,
                    x => x.Id,
                    x => x.Media
                )
                .Unwind<PostAggregate, PostAggregate>(x => x.Media,
                    new AggregateUnwindOptions<PostAggregate> { PreserveNullAndEmptyArrays = true })
                .Lookup<PostAggregate, User, PostAggregate>(userCollection,
                    x => x.UserId,
                    x => x.Id,
                    x => x.User
                )
                .Unwind<PostAggregate, PostAggregate>(x => x.User,
                    new AggregateUnwindOptions<PostAggregate> { PreserveNullAndEmptyArrays = true });

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

        private async Task<List<PostAggregate>> Paginate(IAggregateFluent<PostAggregate> query, ListPostPostQuery request)
        {
            if (request.Cursor != null)
            {
                var filter = request.Asending ? Builders<PostAggregate>.Filter.Gte(x => x.Id, request.Cursor)
                    : Builders<PostAggregate>.Filter.Lte(x => x.Id, request.Cursor);

                query = query.Match(filter);
            }

            return await query.Limit(request.Limit).ToListAsync();
        }

        private async Task<PagingInfo> PreparePagingInfo(IAggregateFluent<PostAggregate> query, ListPostPostQuery request)
        {
            if (request.Cursor != null)
            {
                var previosFilter = request.Asending ? Builders<PostAggregate>.Filter.Lt(x => x.Id, request.Cursor)
                : Builders<PostAggregate>.Filter.Gt(x => x.Id, request.Cursor);

                var nextFilter = request.Asending ? Builders<PostAggregate>.Filter.Gt(x => x.Id, request.Cursor)
                    : Builders<PostAggregate>.Filter.Lt(x => x.Id, request.Cursor);

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

        private IAggregateFluent<PostAggregate> SortQuery(IAggregateFluent<PostAggregate> query, ListPostPostQuery request)
        {
            return request.Asending ? query.SortBy(x => x.Id) : query.SortByDescending(x => x.Id);
        }

        public async Task<Result<PostAggregateDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
        {
            var mediaCollection = _mediaRepository.AsMongoCollection();

            var userCollection = _userRepository.AsMongoCollection();

            var result = await _postRepository.AsMongoCollection().Aggregate()
              .Match(x => x.Id == request.Id)
              .Lookup<Post, Media, PostAggregate>(mediaCollection,
                 x => x.MediaId,
                 x => x.Id,
                 x => x.Media
              )
              .Unwind<PostAggregate, PostAggregate>(x => x.Media)
              .Lookup<PostAggregate, User, PostAggregate>(userCollection,
                  x => x.UserId,
                  x => x.Id,
                  x => x.User
              )
              .Unwind<PostAggregate, PostAggregate>(x => x.User)
              .SingleOrDefaultAsync();

            if (result == null)
            {
                return new Result<PostAggregateDto>(new EntityNotFoundException(typeof(Post), request.Id));
            }


            return await _postResponseFactory.PreparePostAggregateDto(result);
        }
    }
}

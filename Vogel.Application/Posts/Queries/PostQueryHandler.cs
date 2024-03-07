using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Vogel.Application.Common.Exceptions;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Posts.Dtos;
using Vogel.Application.Posts.Factories;
using Vogel.Domain;
using Vogel.Domain.Utils;

namespace Vogel.Application.Posts.Queries
{
    public class PostQueryHandler :
        IApplicationRequestHandler<ListPostPostQuery, List<PostAggregateDto>>,
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

        public async Task<Result<List<PostAggregateDto>>> Handle(ListPostPostQuery request, CancellationToken cancellationToken)
        {
            var mediaCollection = _mediaRepository.AsMongoCollection();

            var userCollection = _userRepository.AsMongoCollection();


            var r = await _postRepository.AsMongoCollection().Aggregate().ToListAsync();

            var result = await _postRepository.AsMongoCollection().Aggregate()             
                .Lookup<Post, Media, PostAggregate>(mediaCollection,
                    x => x.MediaId,
                    x => x.Id,
                    x => x.Media
                )
                .Unwind<PostAggregate, PostAggregate>(x=> x.Media,
                    new AggregateUnwindOptions<PostAggregate> { PreserveNullAndEmptyArrays = true })
                .Lookup<PostAggregate, User, PostAggregate>(userCollection,
                    x => x.UserId,
                    x => x.Id,
                    x => x.User
                )
                .Unwind<PostAggregate, PostAggregate>(x => x.User,
                    new AggregateUnwindOptions<PostAggregate> {PreserveNullAndEmptyArrays =true})
                .ToListAsync();

            return await _postResponseFactory.PrepareListPostAggregateDto(result);
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

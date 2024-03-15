using MongoDB.Driver;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Medias.Dtos;
using Vogel.Application.Posts.Dtos;
using Vogel.Domain;
namespace Vogel.Application.Posts.Factories
{
    public class PostResponseFactory : IPostResponseFactory
    {
        private readonly IMongoDbRepository<Media> _mediaRepository;

        private readonly IMongoDbRepository<Post> _postRepository;

        private readonly IMongoDbRepository<User> _userRepository;

        private readonly IS3ObjectStorageService _s3ObjectStorageService;

        public PostResponseFactory(IMongoDbRepository<Media> mediaRepository, IMongoDbRepository<Post> postRepository, IMongoDbRepository<User> userRepository, IS3ObjectStorageService s3ObjectStorageService)
        {
            _mediaRepository = mediaRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _s3ObjectStorageService = s3ObjectStorageService;
        }

        public async Task<List<PostAggregateDto>> PrepareListPostAggregateDto(List<PostAggregate> posts)
        {
            var tasks = posts.Select(PreparePostAggregateDto);

            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        public async Task<PostAggregateDto> PreparePostAggregateDto(PostAggregate post)
        {
            var result = new PostAggregateDto
            {
                Id = post.Id,
                Caption = post.Caption,
            };

            if(post.Media != null)
            {
                result.Media = new MediaAggregateDto
                {
                    Id = post.Media.Id,
                    MimeType = post.Media.MimeType,
                    MediaType = post.Media.MediaType,
                    UserId = post.Media.UserId,
                    Reference = await _s3ObjectStorageService.GeneratePresignedDownloadUrlAsync(post.Media.File)
                };
            }

            if(post.User != null)
            {
                result.User = new PostUserDto
                {
                    Id = post.Id,
                    FirstName = post.User.FirstName,
                    LastName = post.User.LastName,
                    BirthDate = post.User.BirthDate,
                    Gender = post.User.Gender
                };
            }

            return result;
        }

        public async Task<PostAggregateDto> PreparePostAggregateDto(Post post)
        {
            var userCollection = _userRepository.AsMongoCollection();

            var mediaCollection = _mediaRepository.AsMongoCollection();

            var result = await _postRepository.AsMongoCollection().Aggregate()
                .Match(x => x.Id == post.Id)
                .Lookup<Post, Media, PostAggregate>(mediaCollection,
                    x => x.MediaId,
                    x => x.Id,
                    x => x.Media
                )
                .Unwind<PostAggregate, PostAggregate> (x=> x.Media , 
                    new AggregateUnwindOptions<PostAggregate> { PreserveNullAndEmptyArrays = true })
                .Lookup<PostAggregate, User, PostAggregate>(userCollection,
                    x => x.UserId,
                    x => x.Id,
                    x => x.User
                )
                .Unwind<PostAggregate, PostAggregate>(x => x.User,
                    new AggregateUnwindOptions<PostAggregate> { PreserveNullAndEmptyArrays = true }
                )
                .SingleAsync();


            return await PreparePostAggregateDto(result);
        }


    }
}

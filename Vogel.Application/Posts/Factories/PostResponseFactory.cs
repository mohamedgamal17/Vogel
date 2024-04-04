using MongoDB.Driver;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Medias.Dtos;
using Vogel.Application.Posts.Dtos;
using Vogel.Application.Users.Factories;
using Vogel.Domain;
namespace Vogel.Application.Posts.Factories
{
    public class PostResponseFactory : IPostResponseFactory
    {
        private readonly IMongoDbRepository<Media> _mediaRepository;

        private readonly IMongoDbRepository<Post> _postRepository;

        private readonly IMongoDbRepository<User> _userRepository;

        private readonly IS3ObjectStorageService _s3ObjectStorageService;

        private readonly IUserResponseFactory _userResponseFactory;

        public PostResponseFactory(IMongoDbRepository<Media> mediaRepository, IMongoDbRepository<Post> postRepository, IMongoDbRepository<User> userRepository, IS3ObjectStorageService s3ObjectStorageService, IUserResponseFactory userResponseFactory)
        {
            _mediaRepository = mediaRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _s3ObjectStorageService = s3ObjectStorageService;
            _userResponseFactory = userResponseFactory;
        }

        public async Task<List<PostAggregateDto>> PrepareListPostAggregateDto(List<PostAggregateView> posts)
        {
            var tasks = posts.Select(PreparePostAggregateDto);

            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        public async Task<PostAggregateDto> PreparePostAggregateDto(PostAggregateView post)
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
                result.User = await _userResponseFactory.PreparePublicUserDto(post.User);
            }

            return result;
        }

        public async Task<PostAggregateDto> PreparePostAggregateDto(Post post)
        {
            var userCollection = _userRepository.AsMongoCollection();

            var mediaCollection = _mediaRepository.AsMongoCollection();

            var result = await _postRepository.AsMongoCollection().Aggregate()
                .Match(x => x.Id == post.Id)
                .Lookup<Post, Media, PostAggregateView>(mediaCollection,
                    x => x.MediaId,
                    x => x.Id,
                    x => x.Media
                )
                .Unwind<PostAggregateView, PostAggregateView> (x=> x.Media , 
                    new AggregateUnwindOptions<PostAggregateView> { PreserveNullAndEmptyArrays = true })
                .Lookup<PostAggregateView, User, PostAggregateView>(userCollection,
                    x => x.UserId,
                    x => x.Id,
                    x => x.User
                )
                .Unwind<PostAggregateView, PostAggregateView>(x => x.User,
                    new AggregateUnwindOptions<PostAggregateView> { PreserveNullAndEmptyArrays = true }
                )
                .SingleAsync();


            return await PreparePostAggregateDto(result);
        }


    }
}

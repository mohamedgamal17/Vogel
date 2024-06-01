using MongoDB.Driver;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Medias.Dtos;
using Vogel.Application.Posts.Dtos;
using Vogel.Application.Users.Factories;
using Vogel.Domain.Medias;
using Vogel.Domain.Posts;
using Vogel.Domain.Users;
using Vogel.MongoDb.Entities.Posts;
using Vogel.MongoDb.Entities.Users;
namespace Vogel.Application.Posts.Factories
{
    public class PostResponseFactory : IPostResponseFactory
    {
        private readonly IS3ObjectStorageService _s3ObjectStorageService;

        private readonly IUserResponseFactory _userResponseFactory;

        public PostResponseFactory(IS3ObjectStorageService s3ObjectStorageService, IUserResponseFactory userResponseFactory)
        {
            _s3ObjectStorageService = s3ObjectStorageService;
            _userResponseFactory = userResponseFactory;
        }

        public async Task<List<PostAggregateDto>> PrepareListPostAggregateDto(List<PostMongoView> posts)
        {
            var tasks = posts.Select(PreparePostAggregateDto);

            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        public async Task<PostAggregateDto> PreparePostAggregateDto(PostMongoView post)
        {
            var result = new PostAggregateDto
            {
                Id = post.Id,
                Caption = post.Caption,
                MediaId = post.MediaId,
                UserId = post.UserId
            };

            if (post.Media != null)
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

            if (post.User != null)
            {
                result.User = await _userResponseFactory.PreparePublicUserDto(post.User);
            }

            return result;
        }

        public async Task<PostAggregateDto> PreparePostAggregateDto(Post post, PublicUserMongoView user, Media? media = null)
        {
            var result = new PostAggregateDto
            {
                Id = post.Id,
                Caption = post.Caption,
                MediaId = post.MediaId,
                UserId = post.UserId,
                User = await _userResponseFactory.PreparePublicUserDto(user)
            };

            if (media != null)
            {
                result.Media = new MediaAggregateDto
                {
                    Id = media.Id,
                    MimeType = media.MimeType,
                    MediaType = (MongoDb.Entities.Medias.MediaType)media.MediaType,
                    UserId = media.UserId,
                    Reference = await _s3ObjectStorageService.GeneratePresignedDownloadUrlAsync(media.File)
                };
            }

            return result;
        }
    }
}

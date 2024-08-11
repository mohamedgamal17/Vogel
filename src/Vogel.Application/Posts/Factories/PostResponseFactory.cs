using MongoDB.Driver;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Medias.Dtos;
using Vogel.Application.PostReactions.Dtos;
using Vogel.Application.Posts.Dtos;
using Vogel.Application.Users.Factories;
using Vogel.MongoDb.Entities.Posts;
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

        public async Task<List<PostDto>> PrepareListPostDto(List<PostMongoView> posts)
        {
            var tasks = posts.Select(PreparePostDto);

            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        public async Task<PostDto> PreparePostDto(PostMongoView post)
        {
            var result = new PostDto
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
                result.User = await _userResponseFactory.PrepareUserDto(post.User);
            }

            if(post.ReactionSummary != null)
            {
                result.ReactionSummary = new PostReactionSummaryDto
                {
                    Id = post.Id,
                    TotalLike = post.ReactionSummary.TotalLike,
                    TotalLove = post.ReactionSummary.TotalLove,
                    TotalAngry = post.ReactionSummary.TotalAngry,
                    TotalLaugh = post.ReactionSummary.TotalLaugh,
                    TotalSad = post.ReactionSummary.TotalSad
                };
            }

            return result;
        }

    }
}

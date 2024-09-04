using MongoDB.Driver;
using Vogel.BuildingBlocks.Infrastructure.S3Storage;
using Vogel.BuildingBlocks.Shared.Extensions;
using Vogel.Content.Application.Medias.Dtos;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.Content.Application.Posts.Dtos;
using Vogel.Content.MongoEntities.Posts;
using Vogel.Social.Application.Users.Factories;
using Vogel.Social.Shared.Dtos;
using Vogel.Social.Shared.Services;
namespace Vogel.Content.Application.Posts.Factories
{
    public class PostResponseFactory : IPostResponseFactory
    {
        private readonly IS3ObjectStorageService _s3ObjectStorageService;

        private readonly IUserResponseFactory _userResponseFactory;

        private readonly IUserService _userService;
        public PostResponseFactory(IS3ObjectStorageService s3ObjectStorageService, IUserResponseFactory userResponseFactory, IUserService userService)
        {
            _s3ObjectStorageService = s3ObjectStorageService;
            _userResponseFactory = userResponseFactory;
            _userService = userService;
        }

        public async Task<List<PostDto>> PrepareListPostDto(List<PostMongoView> posts)
        {
            var usersIds = posts.Select(x => x.UserId).ToList();

            var result = await _userService.ListUsersByIds(usersIds, limit: usersIds.Count);

            result.ThrowIfFailure();

            var users = result.Value!.Data.ToDictionary((x) => x.Id, c => c)!;

            var tasks = posts.Select(p=> PreparePostDto(p, users[p.UserId]));

            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        public async Task<PostDto> PreparePostDto(PostMongoView post)
        {
            var userResult = await _userService.GetUserById(post.UserId);

            userResult.ThrowIfFailure();

            return await PreparePostDto(post, userResult.Value!);
        }

        private async Task<PostDto> PreparePostDto(PostMongoView post, UserDto user)
        {

            var result = new PostDto
            {
                Id = post.Id,
                Caption = post.Caption,
                MediaId = post.MediaId,
                UserId = post.UserId,
                User = user
            };

            if (post.Media != null)
            {
                result.Media = new MediaDto
                {
                    Id = post.Media.Id,
                    MimeType = post.Media.MimeType,
                    MediaType = post.Media.MediaType,
                    UserId = post.Media.UserId,
                    Reference = await _s3ObjectStorageService.GeneratePresignedDownloadUrlAsync(post.Media.File)
                };
            }


            if (post.ReactionSummary != null)
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

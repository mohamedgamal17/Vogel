using MongoDB.Driver;
using Vogel.BuildingBlocks.Infrastructure.S3Storage;
using Vogel.BuildingBlocks.Shared.Extensions;
using Vogel.Content.Application.Medias.Dtos;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.Content.Application.Posts.Dtos;
using Vogel.Content.MongoEntities.PostReactions;
using Vogel.Content.MongoEntities.Posts;
using Vogel.Social.Shared.Dtos;
using Vogel.Social.Shared.Services;
namespace Vogel.Content.Application.Posts.Factories
{
    public class PostResponseFactory : IPostResponseFactory
    {
        private readonly IS3ObjectStorageService _s3ObjectStorageService;

        private readonly IUserService _userService;

        private readonly PostReactionMongoRepository _postReactionMongoRepository;
        public PostResponseFactory(IS3ObjectStorageService s3ObjectStorageService, IUserService userService, PostReactionMongoRepository postReactionMongoRepository)
        {
            _s3ObjectStorageService = s3ObjectStorageService;
            _userService = userService;
            _postReactionMongoRepository = postReactionMongoRepository;
        }

        public async Task<List<PostDto>> PrepareListPostDto(List<PostMongoView> posts)
        {
            var usersDictionary = await PrepareDictionaryOfUsers(posts);

            var reactionsDictionary = await PrepareDictionaryOfPostReactionSummary(posts);

            var tasks = posts.Select(post =>
            {
                var user = usersDictionary.GetValueOrDefault(post.UserId);
                var reaction = reactionsDictionary.GetValueOrDefault(post.Id);

                return PreparePostDto(post, user, reaction);

            });

            var result = await Task.WhenAll(tasks);

            return result.ToList();
        }

        public async Task<PostDto> PreparePostDto(PostMongoView post)
        {
            var userResult = await _userService.GetUserById(post.UserId);

            userResult.ThrowIfFailure();

            var reaction = await _postReactionMongoRepository.GetPostReactionSummary(post.Id);

            return await PreparePostDto(post, userResult.Value!, reaction);
        }

        private async Task<PostDto> PreparePostDto(PostMongoView post, UserDto? user = null , PostReactionSummaryMongoView? reactionSummary = null)
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



            if (reactionSummary != null)
            {
                result.ReactionSummary = new PostReactionSummaryDto
                {
                    Id = post.Id,
                    TotalLike = reactionSummary.TotalLike,
                    TotalLove = reactionSummary.TotalLove,
                    TotalAngry = reactionSummary.TotalAngry,
                    TotalLaugh = reactionSummary.TotalLaugh,
                    TotalSad = reactionSummary.TotalSad
                };
            }

            return result;
        }

        private async Task<Dictionary<string , UserDto>> PrepareDictionaryOfUsers(List<PostMongoView> posts)
        {
            var ids = posts.Select(x => x.UserId).ToList();

            var result = await _userService.ListUsersByIds(ids, limit: ids.Count);

            result.ThrowIfFailure();

            return result.Value!.Data.ToDictionary((k) => k.Id, v => v);
        }

        private async Task<Dictionary<string , PostReactionSummaryMongoView>> PrepareDictionaryOfPostReactionSummary(List<PostMongoView> posts)
        {
            var ids = posts.Select(x => x.Id).ToList();

            var summaries = await _postReactionMongoRepository.ListPostsReactionsSummary(ids, limit: ids.Count);

            return summaries.Data.ToDictionary(k=> k.Id , v=> v);
        }
    }
}

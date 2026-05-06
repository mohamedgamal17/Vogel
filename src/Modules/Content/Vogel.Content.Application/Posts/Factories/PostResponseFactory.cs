using MongoDB.Driver;
using Vogel.BuildingBlocks.Shared.Extensions;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.Content.Application.Posts.Dtos;
using Vogel.Content.MongoEntities.PostReactions;
using Vogel.Content.MongoEntities.Posts;
using Vogel.MediaEngine.Shared.Dtos;
using Vogel.MediaEngine.Shared.Services;
using Vogel.Social.Shared.Dtos;
using Vogel.Social.Shared.Services;
namespace Vogel.Content.Application.Posts.Factories
{
    public class PostResponseFactory : IPostResponseFactory
    {
        private readonly IUserService _userService;
        private readonly IMediaService _mediaService;

        private readonly PostReactionMongoRepository _postReactionMongoRepository;
        public PostResponseFactory(IUserService userService, IMediaService mediaService, PostReactionMongoRepository postReactionMongoRepository)
        {
            _userService = userService;
            _mediaService = mediaService;
            _postReactionMongoRepository = postReactionMongoRepository;
        }

        public async Task<List<PostDto>> PrepareListPostDto(List<PostMongoView> posts)
        {
            var usersDictionary = await PrepareDictionaryOfUsers(posts);

            var reactionsDictionary = await PrepareDictionaryOfPostReactionSummary(posts);

            var mediaDictionary = await PrepareDictionaryOfPublicMedia(posts);

            var tasks = posts.Select(post =>
            {
                var user = usersDictionary.GetValueOrDefault(post.UserId);
                var reaction = reactionsDictionary.GetValueOrDefault(post.Id);
                var media = !string.IsNullOrWhiteSpace(post.MediaId) ? mediaDictionary.GetValueOrDefault(post.MediaId) : null;

                return PreparePostDto(post, user, reaction, media);

            });

            var result = await Task.WhenAll(tasks);

            return result.ToList();
        }

        public async Task<PostDto> PreparePostDto(PostMongoView post)
        {
            var userResult = await _userService.GetUserById(post.UserId);

            userResult.ThrowIfFailure();

            var reaction = await _postReactionMongoRepository.GetPostReactionSummary(post.Id);

            PublicMediaFileDto? media = null;
            if (!string.IsNullOrWhiteSpace(post.MediaId))
            {
                var mediaResult = await _mediaService.GetPublicMediaById(post.MediaId);
                if (mediaResult.IsSuccess)
                {
                    media = mediaResult.Value;
                }
            }

            return await PreparePostDto(post, userResult.Value!, reaction, media);
        }

        private Task<PostDto> PreparePostDto(PostMongoView post, UserDto? user = null, PostReactionSummaryMongoView? reactionSummary = null, PublicMediaFileDto? media = null)
        {
            var result = new PostDto
            {
                Id = post.Id,
                Caption = post.Caption,
                UserId = post.UserId,
                User = user,
                MediaId = post.MediaId,
                Media = media,
            };

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

            return Task.FromResult(result);
        }

        private async Task<Dictionary<string , UserDto>> PrepareDictionaryOfUsers(List<PostMongoView> posts)
        {
            var ids = posts.Select(x => x.UserId).ToList();

            var result = await _userService.ListUsersByIds(ids, limit: ids.Count);

            result.ThrowIfFailure();

            return result.Value!.Data.ToDictionary((k) => k.Id, v => v);
        }

        private async Task<Dictionary<string, PublicMediaFileDto>> PrepareDictionaryOfPublicMedia(List<PostMongoView> posts)
        {
            var ids = posts
                .Select(x => x.MediaId)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.Ordinal)
                .ToList()!;

            if (ids.Count == 0)
            {
                return new Dictionary<string, PublicMediaFileDto>(StringComparer.Ordinal);
            }

            var result = await _mediaService.ListPublicMediaByIds(ids!);
            if (result.IsFailure || result.Value == null)
            {
                return new Dictionary<string, PublicMediaFileDto>(StringComparer.Ordinal);
            }

            return result.Value
                .Where(x => !string.IsNullOrWhiteSpace(x.Id))
                .GroupBy(x => x.Id, StringComparer.Ordinal)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);
        }

        private async Task<Dictionary<string , PostReactionSummaryMongoView>> PrepareDictionaryOfPostReactionSummary(List<PostMongoView> posts)
        {
            var ids = posts.Select(x => x.Id).ToList();

            var summaries = await _postReactionMongoRepository.ListPostsReactionsSummary(ids, limit: ids.Count);

            return summaries.Data.ToDictionary(k=> k.Id , v=> v);
        }
    }
}

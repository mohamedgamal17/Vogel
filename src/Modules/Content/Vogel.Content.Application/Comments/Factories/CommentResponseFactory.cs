using Vogel.BuildingBlocks.Shared.Extensions;
using Vogel.Content.Application.CommentReactions.Dtos;
using Vogel.Content.Application.Comments.Dtos;
using Vogel.Content.MongoEntities.CommentReactions;
using Vogel.Content.MongoEntities.Comments;
using Vogel.Social.Shared.Dtos;
using Vogel.Social.Shared.Services;
namespace Vogel.Content.Application.Comments.Factories
{
    public class CommentResponseFactory : ICommentResponseFactory
    {
        private readonly IUserService _userService;
        private readonly CommentReactionMongoRepository _commentReactionMongoRepository;
        public CommentResponseFactory(IUserService userService, CommentReactionMongoRepository commentReactionMongoRepository = null)
        {
            _userService = userService;
            _commentReactionMongoRepository = commentReactionMongoRepository;
        }

        public async Task<List<CommentDto>> PreapreListCommentDto(List<CommentMongoView> comments)
        {
            var usersIds = comments.Select(x => x.UserId).ToList();

            var usersResult = await _userService.ListUsersByIds(usersIds, limit : usersIds.Count);

            usersResult.ThrowIfFailure();

            var usersDictionary  = await PrepareDictionaryOfUsers(comments);

            var reactionsDictionary = await PrepareDictionaryOfReactionSummary(comments);


            return comments.Select(comment =>
            {
                var user = usersDictionary.GetValueOrDefault(comment.UserId);
                var reaction = reactionsDictionary.GetValueOrDefault(comment.Id);
                return PrepareCommentDto(comment, user, reaction);

            }).ToList();
        }
        public async Task<CommentDto> PrepareCommentDto(CommentMongoView comment)
        {
            var userResult = await _userService.GetUserById(comment.UserId);

            userResult.ThrowIfFailure();

            var user = userResult.Value;

            var reaction = await _commentReactionMongoRepository.GetCommentReactionSummary(comment.Id);

            return PrepareCommentDto(comment, user, reaction);
        }

        private CommentDto PrepareCommentDto(CommentMongoView comment , UserDto? user = null , CommentReactionSummaryMongoView? reactionSummary = null)
        {
            var result = new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                PostId = comment.PostId,
                UserId = comment.UserId,
                CommentId = comment.Id,
                User = user,
            };


            if (reactionSummary != null)
            {
                result.ReactionSummary = new CommentReactionSummaryDto
                {
                    Id = comment.Id,
                    TotalLike = reactionSummary.TotalLike,
                    TotalLove = reactionSummary.TotalLove,
                    TotalAngry = reactionSummary.TotalAngry,
                    TotalLaugh = reactionSummary.TotalLaugh,
                    TotalSad = reactionSummary.TotalSad
                };
            }

            return result;
        }


        private async Task<Dictionary<string, UserDto>> PrepareDictionaryOfUsers(List<CommentMongoView> comments)
        {
            var ids = comments.Select(x => x.UserId).ToList();

            var result = await _userService.ListUsersByIds(ids, limit: ids.Count);

            result.ThrowIfFailure();

            return result.Value!.Data.ToDictionary(k => k.Id, v => v);
        }

        private async Task<Dictionary<string, CommentReactionSummaryMongoView>> PrepareDictionaryOfReactionSummary(List<CommentMongoView> comments)
        {
            var ids = comments.Select(x => x.Id).ToList();

            var reactions = await _commentReactionMongoRepository.ListCommentsReactionsSummary(ids, limit: ids.Count);

            return reactions.Data.ToDictionary((k) => k.Id, v => v);
        }
    }
}

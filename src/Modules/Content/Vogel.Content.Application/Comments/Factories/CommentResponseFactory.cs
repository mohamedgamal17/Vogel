using Vogel.BuildingBlocks.Shared.Extensions;
using Vogel.Content.Application.CommentReactions.Dtos;
using Vogel.Content.Application.Comments.Dtos;
using Vogel.Content.MongoEntities.Comments;
using Vogel.Social.Shared.Dtos;
using Vogel.Social.Shared.Services;
namespace Vogel.Content.Application.Comments.Factories
{
    public class CommentResponseFactory : ICommentResponseFactory
    {
        private readonly IUserService _userService;

        public CommentResponseFactory(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<List<CommentDto>> PreapreListCommentDto(List<CommentMongoView> comments)
        {
            var usersIds = comments.Select(x => x.UserId).ToList();

            var usersResult = await _userService.ListUsersByIds(usersIds, limit : usersIds.Count);

            usersResult.ThrowIfFailure();

            var users = usersResult.Value!.Data.ToDictionary((k) => k.Id, v => v);

            return comments.Select(t => PrepareCommentDto(t, users[t.UserId])).ToList();
        }
        public async Task<CommentDto> PrepareCommentDto(CommentMongoView comment)
        {
            var userResult = await _userService.GetUserById(comment.UserId);

            userResult.ThrowIfFailure();
            return PrepareCommentDto(comment, userResult.Value!);
        }

        private CommentDto PrepareCommentDto(CommentMongoView comment , UserDto user)
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


            if (comment.ReactionSummary != null)
            {
                result.ReactionSummary = new CommentReactionSummaryDto
                {
                    Id = comment.Id,
                    TotalLike = comment.ReactionSummary.TotalLike,
                    TotalLove = comment.ReactionSummary.TotalLove,
                    TotalAngry = comment.ReactionSummary.TotalAngry,
                    TotalLaugh = comment.ReactionSummary.TotalLaugh,
                    TotalSad = comment.ReactionSummary.TotalSad
                };
            }

            return result;
        }
    }
}

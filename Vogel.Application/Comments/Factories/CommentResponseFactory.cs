using Vogel.Application.CommentReactions.Dtos;
using Vogel.Application.Comments.Dtos;
using Vogel.Application.Users.Factories;
using Vogel.MongoDb.Entities.Comments;
namespace Vogel.Application.Comments.Factories
{
    public class CommentResponseFactory : ICommentResponseFactory
    {
        private readonly IUserResponseFactory _userFactoryResponse;

        public CommentResponseFactory(IUserResponseFactory userFactoryResponse)
        {
            _userFactoryResponse = userFactoryResponse;
        }

        public async Task<List<CommentAggregateDto>> PreapreListCommentAggregateDto(List<CommentMongoView> comments)
        {
            var tasks = comments.Select(PrepareCommentAggregateDto);

            var result = await Task.WhenAll(tasks);

            return result.ToList();
        }
        public async Task<CommentAggregateDto> PrepareCommentAggregateDto(CommentMongoView comment)
        {
            var result = new CommentAggregateDto
            {
                Id = comment.Id,
                Content = comment.Content,
                PostId = comment.PostId,
                UserId = comment.UserId,
            };

            if(comment.User != null)
            {
                result.User = await _userFactoryResponse.PreparePublicUserDto(comment.User);
            }

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

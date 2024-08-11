using Vogel.Application.CommentReactions.Dtos;
using Vogel.Application.Users.Factories;
using Vogel.MongoDb.Entities.CommentReactions;
namespace Vogel.Application.CommentReactions.Factories
{
    public class CommentReactionResponseFactory : ICommentReactionResponseFactory
    {
        private readonly IUserResponseFactory _userResponseFactory;

        public CommentReactionResponseFactory(IUserResponseFactory userResponseFactory)
        {
            _userResponseFactory = userResponseFactory;
        }

        public async Task<List<CommentReactionDto>> PrepareListCommentReactionDto(List<CommentReactionMongoView> data)
        {
            var tasks = data.Select(PrepareCommentReactionDto);

            return (await Task.WhenAll(tasks)).ToList();
          
        }


        public async Task<CommentReactionDto> PrepareCommentReactionDto(CommentReactionMongoView comment)
        {
            var dto = new CommentReactionDto
            {
                Id = comment.Id,
                CommentId = comment.CommentId,
                UserId = comment.UserId,
                Type = comment.Type,

            };

            if(comment.User != null)
            {
                dto.User = await _userResponseFactory.PrepareUserDto(comment.User);
            }

            return dto;
        }

    }
}

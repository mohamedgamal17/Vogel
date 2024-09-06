using Vogel.BuildingBlocks.Shared.Extensions;
using Vogel.Content.Application.CommentReactions.Dtos;
using Vogel.Content.MongoEntities.CommentReactions;
using Vogel.Social.Shared.Dtos;
using Vogel.Social.Shared.Services;
namespace Vogel.Content.Application.CommentReactions.Factories
{
    public class CommentReactionResponseFactory : ICommentReactionResponseFactory
    {
        private readonly IUserService _userService;

        public CommentReactionResponseFactory(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<List<CommentReactionDto>> PrepareListCommentReactionDto(List<CommentReactionMongoEntity> data)
        {
            var usersIds = data.Select(x => x.UserId).ToList();

            var usersResult = await _userService.ListUsersByIds(usersIds, limit: usersIds.Count);

            usersResult.ThrowIfFailure();

            var users = usersResult.Value!.Data.ToDictionary((k) => k.Id, v => v);

            return data.Select((t)=> PrepareCommentReactionDto(t, users[t.UserId])).ToList();
        }


        public async Task<CommentReactionDto> PrepareCommentReactionDto(CommentReactionMongoEntity reaction)
        {
            var userResult = await _userService.GetUserById(reaction.UserId);

            userResult.ThrowIfFailure();

            return PrepareCommentReactionDto(reaction, userResult.Value!);
        }

        private CommentReactionDto PrepareCommentReactionDto(CommentReactionMongoEntity reaction, UserDto user)
        {
            var result = new CommentReactionDto
            {
                Id = reaction.Id,
                CommentId = reaction.CommentId,
                UserId = reaction.UserId,
                Type = reaction.Type,
                User = user
            };

            return result;
        }

    }
}

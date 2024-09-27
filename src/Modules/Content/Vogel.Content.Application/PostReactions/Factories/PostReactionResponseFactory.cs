using Vogel.BuildingBlocks.Shared.Extensions;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.Content.MongoEntities.PostReactions;
using Vogel.Social.Shared.Dtos;
using Vogel.Social.Shared.Services;
namespace Vogel.Content.Application.PostReactions.Factories
{
    public class PostReactionResponseFactory : IPostReactionResponseFactory
    {

        private readonly IUserService _userService;

        public PostReactionResponseFactory(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<List<PostReactionDto>> PrepareListPostReactionDto(List<PostReactionMongoEntity> data)
        {
            var usersIds = data.Select(x => x.UserId).ToList();

            var usersReult = await _userService.ListUsersByIds(usersIds, limit: usersIds.Count);

            usersReult.ThrowIfFailure();

            var users = usersReult.Value!.Data.ToDictionary((k) => k.Id, v => v);

            return data.Select(t => PreparePostReactionDto(t, users[t.UserId])).ToList();
        }

        public async Task<PostReactionDto> PreparePostReactionDto(PostReactionMongoEntity reaction)
        {
            var userResult = await _userService.GetUserById(reaction.UserId);

            userResult.ThrowIfFailure();

            return PreparePostReactionDto(reaction, userResult.Value!);
        }

        private PostReactionDto PreparePostReactionDto(PostReactionMongoEntity reaction, UserDto user)
        {
            var dto = new PostReactionDto
            {
                Id = reaction.Id,
                PostId = reaction.PostId,
                UserId = reaction.UserId,
                Type = reaction.Type,
                User = user
            };


            return dto;
        }
    }
}

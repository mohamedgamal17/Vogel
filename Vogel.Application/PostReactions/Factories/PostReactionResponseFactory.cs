using Vogel.Application.PostReactions.Dtos;
using Vogel.Application.Users.Factories;
using Vogel.MongoDb.Entities.PostReactions;

namespace Vogel.Application.PostReactions.Factories
{
    public class PostReactionResponseFactory : IPostReactionResponseFactory
    {

        private readonly IUserResponseFactory _userResponseFactory;

        public PostReactionResponseFactory(IUserResponseFactory userResponseFactory)
        {
            _userResponseFactory = userResponseFactory;
        }

        public async Task<List<PostReactionDto>> PreparePostReactionDto(List<PostReactionMongoView> data)
        {
            var tasks = data.Select(PreparePostReactionDto);

            return (await Task.WhenAll(tasks)).ToList();
        }

        public async Task<PostReactionDto> PreparePostReactionDto(PostReactionMongoView reaction)
        {
            var dto = new PostReactionDto
            {
                Id = reaction.Id,
                PostId = reaction.PostId,
                UserId = reaction.UserId,
                Type = reaction.Type
            };

            if(reaction.User != null)
            {
                dto.User = await _userResponseFactory.PreparePublicUserDto(reaction.User);
            }

            return dto;
        }
    }
}

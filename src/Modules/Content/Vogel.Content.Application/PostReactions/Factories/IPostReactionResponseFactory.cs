using Vogel.BuildingBlocks.Application.Factories;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.Content.MongoEntities.PostReactions;
namespace Vogel.Content.Application.PostReactions.Factories
{
    public interface IPostReactionResponseFactory : IResponseFactory
    {
        Task<List<PostReactionDto>> PrepareListPostReactionDto(List<PostReactionMongoEntity> data);
        Task<PostReactionDto> PreparePostReactionDto(PostReactionMongoEntity reaction);
    }
}

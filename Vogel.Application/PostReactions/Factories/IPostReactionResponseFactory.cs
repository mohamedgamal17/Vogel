using Vogel.Application.PostReactions.Dtos;
using Vogel.BuildingBlocks.Application.Factories;
using Vogel.MongoDb.Entities.PostReactions;
namespace Vogel.Application.PostReactions.Factories
{
    public interface IPostReactionResponseFactory : IResponseFactory
    {
        Task<List<PostReactionDto>> PreparePostReactionDto(List<PostReactionMongoView> data);

        Task<PostReactionDto> PreparePostReactionDto(PostReactionMongoView reaction);
    }
}

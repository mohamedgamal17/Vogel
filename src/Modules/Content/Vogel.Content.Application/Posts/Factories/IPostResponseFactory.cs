using Vogel.BuildingBlocks.Application.Factories;
using Vogel.Content.Application.Posts.Dtos;
using Vogel.Content.MongoEntities.Posts;
namespace Vogel.Content.Application.Posts.Factories
{
    public interface IPostResponseFactory : IResponseFactory
    {
        Task<List<PostDto>> PrepareListPostDto(List<PostMongoView> posts);
        Task<PostDto> PreparePostDto(PostMongoView post);
    }
}
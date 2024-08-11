using Vogel.Application.Posts.Dtos;
using Vogel.BuildingBlocks.Application.Factories;
using Vogel.Domain.Medias;
using Vogel.Domain.Posts;
using Vogel.MongoDb.Entities.Posts;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.Application.Posts.Factories
{
    public interface IPostResponseFactory : IResponseFactory
    {
        Task<List<PostDto>> PrepareListPostDto(List<PostMongoView> posts);
        Task<PostDto> PreparePostDto(PostMongoView post);
    }
}
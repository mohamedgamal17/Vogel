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
        Task<List<PostAggregateDto>> PrepareListPostAggregateDto(List<PostMongoView> posts);
        Task<PostAggregateDto> PreparePostAggregateDto(Post post , PublicUserMongoView user , Media? media= null );
        Task<PostAggregateDto> PreparePostAggregateDto(PostMongoView post);
    }
}
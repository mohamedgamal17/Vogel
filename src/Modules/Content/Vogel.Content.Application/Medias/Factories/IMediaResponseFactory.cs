using Vogel.BuildingBlocks.Application.Factories;
using Vogel.Content.Application.Medias.Dtos;
using Vogel.Content.Domain.Medias;
using Vogel.Content.MongoEntities.Medias;
namespace Vogel.Content.Application.Medias.Factories
{
    public interface IMediaResponseFactory : IResponseFactory
    {
        Task<List<MediaDto>> PrepareListMediaDto(List<MediaMongoEntity> medias);
        Task<MediaDto> PrepareMediaDto(Media media);
        Task<MediaDto> PrepareMediaDto(MediaMongoEntity media);
    }
}
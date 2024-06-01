using Vogel.Application.Medias.Dtos;
using Vogel.BuildingBlocks.Application.Factories;
using Vogel.Domain.Medias;
using Vogel.MongoDb.Entities.Medias;

namespace Vogel.Application.Medias.Factories
{
    public interface IMediaResponseFactory : IResponseFactory
    {
        Task<List<MediaAggregateDto>> PrepareListMediaAggregateDto(List<MediaMongoEntity> medias);
        Task<MediaAggregateDto> PrepareMedaiAggregateDto(Media media);
        Task<MediaAggregateDto> PrepareMedaiAggregateDto(MediaMongoEntity media);
    }
}
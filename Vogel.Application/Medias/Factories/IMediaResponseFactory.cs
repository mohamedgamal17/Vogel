using Vogel.Application.Common.Interfaces;
using Vogel.Application.Medias.Dtos;
using Vogel.Domain.Medias;

namespace Vogel.Application.Medias.Factories
{
    public interface IMediaResponseFactory : IResponseFactory
    {
        Task<List<MediaAggregateDto>> PrepareListMediaAggregateDto(List<Media> medias);
        Task<MediaAggregateDto> PrepareMedaiAggregateDto(Media media);
    }
}
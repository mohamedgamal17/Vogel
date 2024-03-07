using Vogel.Application.Common.Interfaces;
using Vogel.Application.Medias.Dtos;
using Vogel.Domain;

namespace Vogel.Application.Medias.Factories
{
    public interface IMediaResponseFactory : IResponseFactory
    {
        Task<List<MediaAggregateDto>> PrepareListMediaAggregateDto(List<MediaLookupDto> medias);
        Task<MediaAggregateDto> PrepareMedaiAggregateDto(Media media);
        Task<MediaAggregateDto> PrepareMedaiAggregateDto(MediaLookupDto media);
    }
}
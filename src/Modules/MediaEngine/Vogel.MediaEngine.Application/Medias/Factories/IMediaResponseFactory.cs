using Vogel.MediaEngine.Domain.Medias;
using Vogel.MediaEngine.MongoEntities.Medias;
using Vogel.MediaEngine.Shared.Dtos;

namespace Vogel.MediaEngine.Application.Medias.Factories
{
    public interface IMediaResponseFactory
    {
        Task<MediaDto> PrepareMediaDto(Media media);
        Task<MediaDto> PrepareMediaDto(MediaMongoEntity media);
        Task<List<MediaDto>> PrepareListMediaDto(List<MediaMongoEntity> medias);
    }
}

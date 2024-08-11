using MongoDB.Driver;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Medias.Dtos;
using Vogel.Domain.Medias;
using Vogel.MongoDb.Entities.Medias;

namespace Vogel.Application.Medias.Factories
{
    public class MediaResponseFactory : IMediaResponseFactory
    {
        private readonly IS3ObjectStorageService _s3ObjectStorageService;
        public MediaResponseFactory(IS3ObjectStorageService s3ObjectStorageService)
        {
            _s3ObjectStorageService = s3ObjectStorageService;
        }

        public async Task<List<MediaAggregateDto>> PrepareListMediaAggregateDto(List<MediaMongoEntity> medias)
        {
            var task = medias.Select(PrepareMedaiAggregateDto);

            var result = await Task.WhenAll(task);

            return result.ToList();
        }

        public async Task<MediaAggregateDto> PrepareMedaiAggregateDto(Media media)
        {
            var result = new MediaAggregateDto
            {
                Id = media.Id,
                MediaType = (MongoDb.Entities.Medias.MediaType) media.MediaType,
                MimeType = media.MimeType,
                Size = media.Size,
                Reference = await _s3ObjectStorageService.GeneratePresignedDownloadUrlAsync(media.File),
                UserId = media.UserId
            };

            return result;
        }

        public async Task<MediaAggregateDto> PrepareMedaiAggregateDto(MediaMongoEntity media)
        {
            var result = new MediaAggregateDto
            {
                Id = media.Id,
                MediaType = media.MediaType,
                MimeType = media.MimeType,
                Size = media.Size,
                Reference = await _s3ObjectStorageService.GeneratePresignedDownloadUrlAsync(media.File),
                UserId = media.UserId
            };
            return result;
        }
    }
}

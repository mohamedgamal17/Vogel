using MongoDB.Driver;
using Vogel.BuildingBlocks.Infrastructure.S3Storage;
using Vogel.Content.Application.Medias.Dtos;
using Vogel.Content.Domain.Medias;
using Vogel.Content.MongoEntities.Medias;
namespace Vogel.Content.Application.Medias.Factories
{
    public class MediaResponseFactory : IMediaResponseFactory
    {
        private readonly IS3ObjectStorageService _s3ObjectStorageService;
        public MediaResponseFactory(IS3ObjectStorageService s3ObjectStorageService)
        {
            _s3ObjectStorageService = s3ObjectStorageService;
        }

        public async Task<List<MediaDto>> PrepareListMediaDto(List<MediaMongoEntity> medias)
        {
            var task = medias.Select(PrepareMediaDto);

            var result = await Task.WhenAll(task);

            return result.ToList();
        }

        public async Task<MediaDto> PrepareMediaDto(Media media)
        {
            var result = new MediaDto
            {
                Id = media.Id,
                MediaType = (MongoEntities.Medias.MediaType)media.MediaType,
                MimeType = media.MimeType,
                Size = media.Size,
                Reference = await _s3ObjectStorageService.GeneratePresignedDownloadUrlAsync(media.File),
                UserId = media.UserId
            };

            return result;
        }

        public async Task<MediaDto> PrepareMediaDto(MediaMongoEntity media)
        {
            var result = new MediaDto
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

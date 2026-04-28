using Vogel.BuildingBlocks.Infrastructure.S3Storage;
using Vogel.MediaEngine.Domain.Medias;
using Vogel.MediaEngine.MongoEntities.Medias;
using Vogel.MediaEngine.Shared.Dtos;

namespace Vogel.MediaEngine.Application.Medias.Factories
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
            var tasks = medias.Select(PrepareMediaDto);
            var result = await Task.WhenAll(tasks);
            return result.ToList();
        }

        public async Task<MediaDto> PrepareMediaDto(Media media)
        {
            return new MediaDto
            {
                Id = media.Id,
                MediaType = (Shared.Enums.MediaType)media.MediaType,
                MimeType = media.MimeType,
                Size = media.Size,
                File = await _s3ObjectStorageService.GeneratePresignedDownloadUrlAsync(media.File),
                UserId = media.UserId,
            };
        }

        public async Task<MediaDto> PrepareMediaDto(MediaMongoEntity media)
        {
            return new MediaDto
            {
                Id = media.Id,
                MediaType = (Shared.Enums.MediaType)media.MediaType,
                MimeType = media.MimeType,
                Size = media.Size,
                File = await _s3ObjectStorageService.GeneratePresignedDownloadUrlAsync(media.File),
                UserId = media.UserId,
            };
        }
    }
}

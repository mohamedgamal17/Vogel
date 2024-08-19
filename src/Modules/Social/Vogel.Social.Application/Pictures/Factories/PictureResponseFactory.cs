using Vogel.BuildingBlocks.Infrastructure.S3Storage;
using Vogel.Social.Domain.Pictures;
using Vogel.Social.MongoEntities.Pictures;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Pictures.Factories
{
    public class PictureResponseFactory : IPictureResponseFactory
    {
        private readonly IS3ObjectStorageService _s3ObjectStorageService;

        public PictureResponseFactory(IS3ObjectStorageService s3ObjectStorageService)
        {
            _s3ObjectStorageService = s3ObjectStorageService;
        }

        public async Task<PictureDto> PreparePictureDto(Picture picture)
        {
            var result = new PictureDto
            {
                Id = picture.Id,
                Reference = await _s3ObjectStorageService.GeneratePresignedDownloadUrlAsync(picture.File),
                UserId = picture.UserId
            };

            return result;
        }

        public async Task<PictureDto> PreparePictureDto(PictureMongoEntity picture)
        {
            var result = new PictureDto
            {
                Id = picture.Id,
                Reference = await _s3ObjectStorageService.GeneratePresignedDownloadUrlAsync(picture.File),
                UserId = picture.UserId
            };

            return result;
        }
    }
}

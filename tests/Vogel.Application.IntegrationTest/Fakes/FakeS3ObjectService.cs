using Vogel.Application.Common.Interfaces;
using Vogel.Application.Common.Models;
using Vogel.BuildingBlocks.Infrastructure.S3Storage;

namespace Vogel.Application.IntegrationTest.Fakes
{
    public class FakeS3ObjectService : IS3ObjectStorageService
    {
        public Task<string> GeneratePresignedDownloadUrlAsync(string objectName, int expiry = 604800)
        {
            string url = "https://www.fake.com/";

            return Task.FromResult(url + objectName);
        }

        public Task RemoveObjectAsync(string objectName)
        {
            return Task.CompletedTask;
        }

        public Task<S3ObjectStorageSaveResponseModel> SaveObjectAsync(S3ObjectStorageSaveModel model)
        {
            var response = new S3ObjectStorageSaveResponseModel
            {
                ObjectName = model.FileName,
                Size = model.Content.Length
            };

            return Task.FromResult(response);
        }
    }
}

using Vogel.Application.Common.Models;

namespace Vogel.Application.Common.Interfaces
{
    public interface IS3ObjectStorageService
    {
        Task<S3ObjectStorageSaveResponseModel> SaveObjectAsync(S3ObjectStorageSaveModel model);

        Task<string> GeneratePresignedDownloadUrlAsync(string objectName , int expiry = 604800);

        Task RemoveObjectAsync(string objectName);
    }
}

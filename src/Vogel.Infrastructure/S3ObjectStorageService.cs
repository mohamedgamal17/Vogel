using Minio;
using Minio.DataModel.Args;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Common.Models;

namespace Vogel.Infrastructure
{
    public class S3ObjectStorageService : IS3ObjectStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly S3ObjectStorageConfiguration _s3ObjectStorageConfiguration;
        public S3ObjectStorageService(IMinioClient minioClient, S3ObjectStorageConfiguration s3ObjectStorageConfiguration)
        {
            _s3ObjectStorageConfiguration = s3ObjectStorageConfiguration;
            _minioClient = GenerateMinioClient(s3ObjectStorageConfiguration);
        }

        public async Task<S3ObjectStorageSaveResponseModel> SaveObjectAsync(S3ObjectStorageSaveModel model )
        {
            bool isBucketExist = await IsBucketExist();

            if (!isBucketExist)
            {
                await CreateBucketAsync();
            }

            var request = new PutObjectArgs()
                .WithBucket(_s3ObjectStorageConfiguration.BucketName)
                .WithObject(model.FileName)
                .WithContentType(model.ContentType)
                .WithStreamData(model.Content)
                .WithObjectSize(model.Content.Length);

            var response = await _minioClient.PutObjectAsync(request);

            var result = new S3ObjectStorageSaveResponseModel
            {
                ObjectName = response.ObjectName,
                Size = response.Size
            };

            return result;
        }

        public async Task<string> GeneratePresignedDownloadUrlAsync(string objectName , int expiry = 604800)
        {
            var request = new PresignedGetObjectArgs()
                .WithBucket(_s3ObjectStorageConfiguration.BucketName)
                .WithObject(objectName)
                .WithExpiry(expiry);

            var response = await _minioClient.PresignedGetObjectAsync(request);

            return response;
        } 

        public async Task RemoveObjectAsync(string objectName)
        {
            var request = new RemoveObjectArgs()
                .WithBucket(_s3ObjectStorageConfiguration.BucketName)
                .WithObject(objectName);

            await _minioClient.RemoveObjectAsync(request);
        }
        private async Task<bool> IsBucketExist()
        {
            var args = new BucketExistsArgs()
                .WithBucket(_s3ObjectStorageConfiguration.BucketName);

            return await _minioClient.BucketExistsAsync(args);
        }

        private async Task CreateBucketAsync()
        {
            var args = new MakeBucketArgs()
                .WithBucket(_s3ObjectStorageConfiguration.BucketName);

            await _minioClient.MakeBucketAsync(args);
                
        }

        private IMinioClient GenerateMinioClient(S3ObjectStorageConfiguration configuration)
        {
            var client = new MinioClient()
                .WithEndpoint(configuration.EndPoint)
                .WithCredentials(configuration.AccessKey, configuration.SecretKey);

            if (configuration.WithSSL)
            {
                client = client.WithSSL();
            }

            return client.Build();
        }


    }
}

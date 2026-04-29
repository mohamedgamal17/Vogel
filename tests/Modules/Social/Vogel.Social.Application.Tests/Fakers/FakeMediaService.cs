using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.MediaEngine.Shared.Dtos;
using Vogel.MediaEngine.Shared.Enums;
using Vogel.MediaEngine.Shared.Services;
using Vogel.Social.Infrastructure.EntityFramework;

namespace Vogel.Social.Application.Tests.Fakers
{
    public class FakeMediaService : IMediaService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, PublicMediaFileDto> _medias = new();

        public FakeMediaService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<Result<MediaDto>> GetMediaById(string id)
        {
            if (!_medias.TryGetValue(id, out var media))
            {
                return Task.FromResult(new Result<MediaDto>(new EntityNotFoundException(typeof(PublicMediaFileDto), id)));
            }

            var securityContext = _serviceProvider.GetRequiredService<ISecurityContext>();
            var currentUserId = securityContext.User?.Id;

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Task.FromResult(new Result<MediaDto>(new UnauthorizedAccessException()));
            }

            if (media.UserId != currentUserId)
            {
                return Task.FromResult(new Result<MediaDto>(new ForbiddenAccessException()));
            }

            return Task.FromResult<Result<MediaDto>>(new MediaDto
            {
                Id = media.Id,
                File = media.File,
                MediaType = media.MediaType,
                MimeType = media.MimeType,
                Size = media.Size,
                UserId = media.UserId
            });
        }

        public Task<Result<PublicMediaFileDto>> GetPublicMediaById(string id)
        {
            if (!_medias.TryGetValue(id, out var media))
            {
                return Task.FromResult(new Result<PublicMediaFileDto>(new EntityNotFoundException(typeof(PublicMediaFileDto), id)));
            }

            return Task.FromResult<Result<PublicMediaFileDto>>(new PublicMediaFileDto
            {
                Id = media.Id,
                File = media.File,
                Reference = media.Reference,
                MediaType = media.MediaType,
                MimeType = media.MimeType,
                Size = media.Size,
                UserId = media.UserId
            });
        }

        public async Task<Result<List<MediaDto>>> ListMediaByIds(List<string> ids)
        {
            var tasks = ids.Select(GetMediaById);
            var results = await Task.WhenAll(tasks);
            var firstFailure = results.FirstOrDefault(x => x.IsFailure);
            if (firstFailure != null)
            {
                return new Result<List<MediaDto>>(firstFailure.Exception!);
            }

            return results.Select(x => x.Value!).ToList();
        }

        public async Task<Result<List<PublicMediaFileDto>>> ListPublicMediaByIds(List<string> ids)
        {
            var tasks = ids.Select(GetPublicMediaById);
            var results = await Task.WhenAll(tasks);
            var firstFailure = results.FirstOrDefault(x => x.IsFailure);
            if (firstFailure != null)
            {
                return new Result<List<PublicMediaFileDto>>(firstFailure.Exception!);
            }

            return results.Select(x => x.Value!).ToList();
        }

        public PublicMediaFileDto AddMedia(string userId)
        {
            return AddMedia(userId, MediaType.Image);
        }

        public PublicMediaFileDto AddMedia(string userId, MediaType mediaType)
        {
            var media = new PublicMediaFileDto
            {
                Id = Guid.NewGuid().ToString(),
                File = Guid.NewGuid().ToString(),
                Reference = Guid.NewGuid().ToString(),
                MimeType = mediaType == MediaType.Image ? "image/png" : "video/mp4",
                Size = 123,
                UserId = userId,
                MediaType = mediaType
            };

            _medias[media.Id] = media;

            SeedLegacyPictureIfExists(media);

            return media;
        }

        public void Reset()
        {
            _medias.Clear();
        }

        private void SeedLegacyPictureIfExists(PublicMediaFileDto media)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SocialDbContext>();
            var hasLegacyPictureTable = dbContext.Database
                .SqlQueryRaw<int>("SELECT CAST(COUNT(1) AS int) AS [Value] FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'Social' AND TABLE_NAME = 'Pictures'")
                .Single() > 0;

            if (!hasLegacyPictureTable)
            {
                return;
            }

            dbContext.Database.ExecuteSqlInterpolated($@"
IF NOT EXISTS (SELECT 1 FROM [Social].[Pictures] WHERE [Id] = {media.Id})
BEGIN
    INSERT INTO [Social].[Pictures] ([Id], [File], [UserId], [CreatorId], [CreationTime], [ModificationTime], [ModifierId], [DeletionTime], [DeletorId])
    VALUES ({media.Id}, {media.File}, {media.UserId}, NULL, {DateTime.UtcNow}, {DateTime.UtcNow}, NULL, NULL, NULL)
END");
        }
    }
}

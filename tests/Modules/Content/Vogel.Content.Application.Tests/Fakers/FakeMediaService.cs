using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Microsoft.Extensions.DependencyInjection;
using Vogel.MediaEngine.Shared.Dtos;
using Vogel.MediaEngine.Shared.Enums;
using Vogel.MediaEngine.Shared.Services;

namespace Vogel.Content.Application.Tests.Fakers
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
            var media = new PublicMediaFileDto
            {
                Id = Guid.NewGuid().ToString(),
                File = Guid.NewGuid().ToString(),
                Reference = Guid.NewGuid().ToString(),
                MimeType = "image/png",
                Size = 123,
                UserId = userId,
                MediaType = MediaType.Image
            };

            _medias[media.Id] = media;
            return media;
        }

        public List<PublicMediaFileDto> AddMedias(string userId, int count)
        {
            var medias = new List<PublicMediaFileDto>(count);
            for (var i = 0; i < count; i++)
            {
                medias.Add(AddMedia(userId));
            }

            return medias;
        }

        public void Reset()
        {
            _medias.Clear();
        }
    }
}

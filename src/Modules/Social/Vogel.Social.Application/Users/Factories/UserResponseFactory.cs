using Vogel.BuildingBlocks.Shared.Extensions;
using Vogel.MediaEngine.Shared.Dtos;
using Vogel.MediaEngine.Shared.Services;
using Vogel.Social.MongoEntities.Users;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Users.Factories
{
    public class UserResponseFactory : IUserResponseFactory
    {
        private readonly IMediaService _mediaService;

        public UserResponseFactory(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        public async Task<List<UserDto>> PrepareListUserDto(List<UserMongoView> users)
        {
            var mediaById = await PreparePublicMediaDictionary(users);

            var tasks = users.Select(user => PrepareUserDtoFromDictionary(user, mediaById));

            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        public async Task<UserDto> PrepareUserDto(UserMongoView user)
        {
            var avatar = await TryGetPublicMedia(user.AvatarId);
            var cover = await TryGetPublicMedia(user.CoverId);

            var result = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                BirthDate = user.BirthDate.ToShortDateString(),
                AvatarId = user.AvatarId,
                Avatar = avatar,
                CoverId = user.CoverId,
                Cover = cover,
            };

            return result;
        }

        private Task<UserDto> PrepareUserDtoFromDictionary(UserMongoView user, IReadOnlyDictionary<string, PublicMediaFileDto> mediaById)
        {
            mediaById.TryGetValue(user.AvatarId ?? string.Empty, out var avatar);
            mediaById.TryGetValue(user.CoverId ?? string.Empty, out var cover);

            var result = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                BirthDate = user.BirthDate.ToShortDateString(),
                AvatarId = user.AvatarId,
                Avatar = avatar,
                CoverId = user.CoverId,
                Cover = cover,
            };

            return Task.FromResult(result);
        }

        private async Task<PublicMediaFileDto?> TryGetPublicMedia(string? mediaId)
        {
            if (string.IsNullOrWhiteSpace(mediaId))
            {
                return null;
            }

            var result = await _mediaService.GetPublicMediaById(mediaId);
            return result.IsSuccess ? result.Value : null;
        }

        private async Task<IReadOnlyDictionary<string, PublicMediaFileDto>> PreparePublicMediaDictionary(IEnumerable<UserMongoView> users)
        {
            var ids = users
                .SelectMany(x => new[] { x.AvatarId, x.CoverId })
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.Ordinal)
                .ToList()!;

            if (ids.Count == 0)
            {
                return new Dictionary<string, PublicMediaFileDto>(StringComparer.Ordinal);
            }

            var result = await _mediaService.ListPublicMediaByIds(ids!);
            if (result.IsFailure || result.Value == null)
            {
                return new Dictionary<string, PublicMediaFileDto>(StringComparer.Ordinal);
            }

            return result.Value
                .Where(x => !string.IsNullOrWhiteSpace(x.Id))
                .GroupBy(x => x.Id, StringComparer.Ordinal)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);
        }
    }
}
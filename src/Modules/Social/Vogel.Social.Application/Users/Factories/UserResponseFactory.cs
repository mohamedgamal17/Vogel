using Vogel.BuildingBlocks.Infrastructure.S3Storage;
using Vogel.Social.MongoEntities.Users;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Users.Factories
{
    public class UserResponseFactory : IUserResponseFactory
    {
        private readonly IS3ObjectStorageService _s3ObjectStorageService;

        public UserResponseFactory(IS3ObjectStorageService s3ObjectStorageService)
        {
            _s3ObjectStorageService = s3ObjectStorageService;
        }

        public async Task<List<UserDto>> PrepareListUserDto(List<UserMongoView> users)
        {
            var tasks = users.Select(PrepareUserDto);

            var results = await Task.WhenAll(tasks);

            return results.ToList();
        }

        public async Task<UserDto> PrepareUserDto(UserMongoView user)
        {
            var result = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                AvatarId = user.AvatarId,
                BirthDate = user.BirthDate.ToShortDateString()
            };

            if (user.Avatar != null)
            {
                result.Avatar = new PictureDto
                {
                    Id = user.Avatar.Id,
                    UserId = user.Avatar.UserId,
                    Reference = await _s3ObjectStorageService.GeneratePresignedDownloadUrlAsync(user.Avatar.File)
                };
            }

            return result;
        }
    }
}
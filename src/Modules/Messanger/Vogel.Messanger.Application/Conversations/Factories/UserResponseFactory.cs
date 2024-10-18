using Vogel.BuildingBlocks.Infrastructure.S3Storage;
using Vogel.Messanger.MongoEntities.Users;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Messanger.Application.Conversations.Factories
{
    public class UserResponseFactory : IUserResponseFactory
    {
        private readonly IS3ObjectStorageService _s3ObjectStorageService;

        public UserResponseFactory(IS3ObjectStorageService s3ObjectStorageService)
        {
            _s3ObjectStorageService = s3ObjectStorageService;
        }

        public async Task<List<UserDto>> PrepareListUserDto(List<UserMongoEntity> users)
        {
            var userTasks = users.Select(PreapreUserDto);

            var results = await Task.WhenAll(userTasks);

            return results.ToList();
        }

        public async Task<UserDto> PreapreUserDto(UserMongoEntity user)
        {
            var dto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                AvatarId = user.AvatarId,
                BirthDate = user.BirthDate.ToShortDateString(),
            };

            if(user.Avatar != null)
            {
                dto.Avatar = new PictureDto
                {
                    Id = user.Avatar.Id,
                    UserId = user.Avatar.UserId,
                    Reference = await _s3ObjectStorageService.GeneratePresignedDownloadUrlAsync(user.Avatar.File)
                };
            }

            return dto;
        }

    }


}

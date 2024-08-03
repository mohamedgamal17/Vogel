using MongoDB.Driver;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Medias.Dtos;
using Vogel.Application.Users.Dtos;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.Application.Users.Factories
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

            var results =  await Task.WhenAll(tasks);

            return results.ToList();
        }    
   
        public async Task<UserDto> PrepareUserDto(UserMongoView user)
        {
            var result = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender =  user.Gender,
                AvatarId = user.AvatarId,
                BirthDate = user.BirthDate.ToShortDateString()
            };

            if (user.Avatar != null)
            {
                result.Avatar = new MediaAggregateDto
                {
                    Id = user.Avatar.Id,
                    UserId = user.Avatar.UserId,
                    MediaType = user.Avatar.MediaType,
                    MimeType = user.Avatar.MimeType,
                    Size = user.Avatar.Size,
                    Reference = await _s3ObjectStorageService.GeneratePresignedDownloadUrlAsync(user.Avatar.File)
                };
            }

            return result;
        }
    }
}

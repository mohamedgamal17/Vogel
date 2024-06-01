using MongoDB.Driver;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Medias.Dtos;
using Vogel.Application.Users.Dtos;
using Vogel.Domain;
using Vogel.Domain.Medias;
using Vogel.Domain.Users;
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

        public async Task<List<UserDto>> PrepareListUserAggregateDto(List<UserMongoView> users)
        {
            var tasks = users.Select(PrepareUserAggregateDto);

            var results =  await Task.WhenAll(tasks);

            return results.ToList();
        }    
        public async Task<UserDto> PrepareUserAggregateDto(UserAggregate user , Media? avatar = null)
        {
            var result = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = (MongoDb.Entities.Users.Gender)user.Gender,
                AvatarId = user.AvatarId,
                BirthDate = user.BirthDate.ToShortDateString()
            };

            if (avatar != null)
            {
                result.Avatar = new MediaAggregateDto
                {
                    Id = avatar.Id,
                    UserId = avatar.UserId,
                    MediaType = (MongoDb.Entities.Medias.MediaType)avatar.MediaType,
                    MimeType = avatar.MimeType,
                    Size = avatar.Size,
                    Reference = await _s3ObjectStorageService.GeneratePresignedDownloadUrlAsync(avatar.File)
                };
            }

            return result;
        }
        public async Task<UserDto> PrepareUserAggregateDto(UserMongoView user)
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
        public async Task<PublicUserDto> PreparePublicUserDto(PublicUserMongoView user)
        {
            var result = new PublicUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate.ToShortDateString(),
                Gender = user.Gender,
                AvatarId = user.AvatarId,

            };

            if(user.Avatar != null)
            {
                result.Avatar = new MediaAggregateDto
                {
                    Id = user.Avatar!.Id,
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

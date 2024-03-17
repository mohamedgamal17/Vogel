using MongoDB.Driver;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Medias.Dtos;
using Vogel.Application.Users.Dtos;
using Vogel.Domain;

namespace Vogel.Application.Users.Factories
{
    public class UserResponseFactory : IUserResponseFactory
    {
        private readonly IMongoDbRepository<User> _userRepository;
        private readonly IMongoDbRepository<Media> _mediaRepository;
        private readonly IS3ObjectStorageService _s3ObjectStorageService;

        public UserResponseFactory(IMongoDbRepository<User> userRepository, IMongoDbRepository<Media> mediaRepository, IS3ObjectStorageService s3ObjectStorageService)
        {
            _userRepository = userRepository;
            _mediaRepository = mediaRepository;
            _s3ObjectStorageService = s3ObjectStorageService;
        }

        public async Task<List<UserAggregateDto>> PrepareListUserAggregateDto(List<UserAggregate> users)
        {
            var tasks = users.Select(PrepareUserAggregateDto);

            var results =  await Task.WhenAll(tasks);

            return results.ToList();
        }

        public async Task<UserAggregateDto> PrepareUserAggregateDto(UserAggregate user)
        {
            var result = new UserAggregateDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                AvatarId = user.AvatarId,
                BirthDate = user.BirthDate
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

        public async Task<UserAggregateDto> PrepareUserAggregateDto(User user)
        {
            var mediaCollection = _mediaRepository.AsMongoCollection();

            var result  = await _userRepository.AsMongoCollection()
                .Aggregate()
                .Match(Builders<User>.Filter.Eq(x => x.Id, user.Id))
                .Lookup<User, Media, UserAggregate>(mediaCollection,
                    x => x.AvatarId,
                    y => y.Id,
                    x => x.Avatar
                ).Unwind<UserAggregate, UserAggregate>(x => x.Avatar, new AggregateUnwindOptions<UserAggregate> { PreserveNullAndEmptyArrays = true })                
                .FirstAsync();

            return await PrepareUserAggregateDto(result);
        }
    }
}

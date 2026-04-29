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

            if (user.AvatarId != null)
            {
                var avatarResult = await _mediaService.GetPublicMediaById(user.AvatarId);
                if (avatarResult.IsSuccess)
                {
                    result.Avatar = avatarResult.Value;
                }
            }

            return result;
        }
    }
}
using Vogel.Social.Application.Pictures.Factories;
using Vogel.Social.MongoEntities.Pictures;
using Vogel.Social.MongoEntities.Users;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Users.Factories
{
    public class UserResponseFactory : IUserResponseFactory
    {
        private readonly PictureMongoRepository _pictureMongoRepository;
        private readonly IPictureResponseFactory _pictureResponseFactory;

        public UserResponseFactory(PictureMongoRepository pictureMongoRepository, IPictureResponseFactory pictureResponseFactory)
        {
            _pictureMongoRepository = pictureMongoRepository;
            _pictureResponseFactory = pictureResponseFactory;
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
                var avatar = await _pictureMongoRepository.FindByIdAsync(user.AvatarId);
                if (avatar != null)
                {
                    result.Avatar = await _pictureResponseFactory.PreparePictureDto(avatar);
                }
            }

            return result;
        }
    }
}
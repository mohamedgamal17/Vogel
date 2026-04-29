using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.MediaEngine.Shared.Enums;
using Vogel.MediaEngine.Shared.Services;
using Vogel.Social.Application.Users.Factories;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Users;
using Vogel.Social.MongoEntities.Users;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Social.Application.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IApplicationRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly ISocialRepository<User> _userRepository;
        private readonly ISecurityContext _securityContext;
        private readonly UserMongoRepository _userMongoRepository;
        private readonly IUserResponseFactory _userResponseFactory;
        private readonly IMediaService _mediaService;

        public CreateUserCommandHandler(ISocialRepository<User> userRepository, ISecurityContext securityContext, UserMongoRepository userMongoRepository, IUserResponseFactory userResponseFactory, IMediaService mediaService)
        {
            _userRepository = userRepository;
            _securityContext = securityContext;
            _userMongoRepository = userMongoRepository;
            _userResponseFactory = userResponseFactory;
            _mediaService = mediaService;
        }

        public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            string userId = _securityContext.User!.Id;

            bool isUserExist = (await _userRepository.FindByIdAsync(userId)) != null;

            if (isUserExist)
            {
                return new Result<UserDto>(new BusinessLogicException("User already has profile"));
            }

            string? avatarId = null;

            if (request.AvatarId != null)
            {
                var mediaResult = await _mediaService.GetMediaById(request.AvatarId);
                if (mediaResult.IsFailure)
                {
                    return new Result<UserDto>(mediaResult.Exception!);
                }

                if (mediaResult.Value!.MediaType != MediaType.Image)
                {
                    return new Result<UserDto>(new BusinessLogicException("Avatar must be an image"));
                }

                avatarId = mediaResult.Value!.Id;
            }

            var user = new User(userId)
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                AvatarId = avatarId,
                BirthDate = request.BirthDate,
                Gender = request.Gender,
            };

            await _userRepository.InsertAsync(user);

            var userView = await _userMongoRepository.GetByIdUserMongoView(user.Id);

            return await _userResponseFactory.PrepareUserDto(userView!);
        }
    }
}

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

namespace Vogel.Social.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IApplicationRequestHandler<UpdateUserCommand, UserDto>
    {
        private readonly ISocialRepository<User> _userRepository;
        private readonly ISecurityContext _securityContext;
        private readonly UserMongoRepository _userMongoRepository;
        private readonly IUserResponseFactory _userResponseFactory;
        private readonly IMediaService _mediaService;

        public UpdateUserCommandHandler(ISocialRepository<User> userRepository, ISecurityContext securityContext, UserMongoRepository userMongoRepository, IUserResponseFactory userResponseFactory, IMediaService mediaService)
        {
            _userRepository = userRepository;
            _securityContext = securityContext;
            _userMongoRepository = userMongoRepository;
            _userResponseFactory = userResponseFactory;
            _mediaService = mediaService;
        }

        public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            string currentUserId = _securityContext.User!.Id!;

            var user = await _userRepository.FindByIdAsync(currentUserId);

            if (user == null)
            {
                return new Result<UserDto>(new EntityNotFoundException(typeof(User), currentUserId));
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

            PrepareUserEntity(request, user, avatarId);

            await _userRepository.UpdateAsync(user);

            var userView = await _userMongoRepository.GetByIdUserMongoView(user.Id);

            return await _userResponseFactory.PrepareUserDto(userView);
        }

        private static void PrepareUserEntity(UpdateUserCommand command, User user, string? avatarId = null)
        {
            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.BirthDate = command.BirthDate;
            user.Gender = command.Gender;
            user.AvatarId = avatarId;
        }
    }
}

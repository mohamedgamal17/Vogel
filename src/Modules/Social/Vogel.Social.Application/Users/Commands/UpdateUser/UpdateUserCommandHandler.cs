using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Pictures.Policies;
using Vogel.Social.Application.Users.Factories;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Pictures;
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
        private readonly ISocialRepository<Picture> _pictureRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public UpdateUserCommandHandler(ISocialRepository<User> userRepository, ISecurityContext securityContext, UserMongoRepository userMongoRepository, IUserResponseFactory userResponseFactory, ISocialRepository<Picture> pictureRepository, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _userRepository = userRepository;
            _securityContext = securityContext;
            _userMongoRepository = userMongoRepository;
            _userResponseFactory = userResponseFactory;
            _pictureRepository = pictureRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            string currentUserId = _securityContext.User!.Id!;

            var user = await _userRepository.FindByIdAsync(currentUserId);

            if (user == null)
            {
                return new Result<UserDto>(new EntityNotFoundException(typeof(User), currentUserId));
            }

            if (request.AvatarId != null)
            {
                var avatar = await _pictureRepository.FindByIdAsync(request.AvatarId);

                var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(avatar!, PictureOperationRequirements.IsPictureOwner);

                if (authorizationResult.IsFailure)
                {
                    return new Result<UserDto>(authorizationResult.Exception!);
                }
            }

            PrepareUserEntity(request, user);

            await _userRepository.UpdateAsync(user);

            var userView = await _userMongoRepository.GetByIdUserMongoView(user.Id);

            return await _userResponseFactory.PrepareUserDto(userView);
        }

        private void PrepareUserEntity(UpdateUserCommand command, User user)
        {
            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.BirthDate = command.BirthDate;
            user.Gender = command.Gender;
            user.AvatarId = command.AvatarId;
        }
    }
}

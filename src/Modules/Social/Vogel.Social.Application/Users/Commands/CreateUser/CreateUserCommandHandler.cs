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
namespace Vogel.Social.Application.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IApplicationRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly ISocialRepository<User> _userRepository;
        private readonly ISecurityContext _securityContext;
        private readonly UserMongoRepository _userMongoRepository;
        private readonly IUserResponseFactory _userResponseFactory;
        private readonly ISocialRepository<Picture> _pictureRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public CreateUserCommandHandler(ISocialRepository<User> userRepository, ISecurityContext securityContext, UserMongoRepository userMongoRepository, IUserResponseFactory userResponseFactory, ISocialRepository<Picture> pictureRepository, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _userRepository = userRepository;
            _securityContext = securityContext;
            _userMongoRepository = userMongoRepository;
            _userResponseFactory = userResponseFactory;
            _pictureRepository = pictureRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            string userId = _securityContext.User!.Id;

            bool isUserExist = (await _userRepository.FindByIdAsync(userId)) != null;

            if (isUserExist)
            {
                return new Result<UserDto>(new BusinessLogicException("User already has profile"));
            }

            Picture? avatar = null;

            if (request.AvatarId != null)
            {
                avatar = await _pictureRepository.FindByIdAsync(request.AvatarId);

                if (avatar == null)
                {
                    return new Result<UserDto>(new EntityNotFoundException(typeof(Picture), request.AvatarId));
                }

                var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(avatar!, PictureOperationRequirements.IsPictureOwner);

                if (authorizationResult.IsFailure)
                {
                    return new Result<UserDto>(authorizationResult.Exception!);
                }
            }
            var user = new User(userId)
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                AvatarId = avatar?.Id,
                BirthDate = request.BirthDate,
                Gender = request.Gender,
            };

            await _userRepository.InsertAsync(user);

            var userView = await _userMongoRepository.GetByIdUserMongoView(user.Id);

            return await _userResponseFactory.PrepareUserDto(userView!);
        }
    }
}

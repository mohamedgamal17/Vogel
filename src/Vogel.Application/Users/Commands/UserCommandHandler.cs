using Vogel.Application.Medias.Policies;
using Vogel.Application.Users.Dtos;
using Vogel.Application.Users.Factories;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Application.Security;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Repositories;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Domain.Medias;
using Vogel.Domain.Users;
using Vogel.MongoDb.Entities.Users;
namespace Vogel.Application.Users.Commands
{
    public class UserCommandHandler : 
        IApplicationRequestHandler<CreateUserCommand, UserDto>,
        IApplicationRequestHandler<UpdateUserCommand , UserDto>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Media> _mediaRepository;
        private readonly UserMongoRepository _userMongoRepository;
        private readonly IUserResponseFactory _userResponseFactory;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly ISecurityContext _securityContext;

        public UserCommandHandler(IRepository<User> userRepository, IRepository<Media> mediaRepository, UserMongoRepository userMongoRepository, IUserResponseFactory userResponseFactory, IApplicationAuthorizationService applicationAuthorizationService, ISecurityContext securityContext)
        {
            _userRepository = userRepository;
            _mediaRepository = mediaRepository;
            _userMongoRepository = userMongoRepository;
            _userResponseFactory = userResponseFactory;
            _applicationAuthorizationService = applicationAuthorizationService;
            _securityContext = securityContext;
        }

        public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            string userId = _securityContext.User!.Id;

            bool isUserExist = (await _userRepository.FindByIdAsync(userId)) != null;

            if (isUserExist)
            {
                return new Result<UserDto>(new BusinessLogicException("User already has profile"));
            }

            var user = new User(userId);

            Media? avatar = null;

            if (request.AvatarId != null)
            {
                 avatar = await _mediaRepository.FindByIdAsync(request.AvatarId);

                var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(avatar!, MediaOperationRequirements.IsOwner);

                if (authorizationResult.IsFailure)
                {
                    return new Result<UserDto>(authorizationResult.Exception!);
                }
            }

            PrepareUserEntity(request, user);

            await _userRepository.InsertAsync(user);

            var userView = await _userMongoRepository.GetByIdUserMongoView(user.Id);

            return await _userResponseFactory.PrepareUserDto(userView);
        }

        public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            string currentUserId = _securityContext.User!.Id!;

            var user = await _userRepository.FindByIdAsync(currentUserId);

            if(user == null)
            {
                return new Result<UserDto>(new EntityNotFoundException(typeof(User), currentUserId));
            }

            if (request.AvatarId != null)
            {
                var avatar = await _mediaRepository.FindByIdAsync(request.AvatarId);

                var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(avatar!, MediaOperationRequirements.IsOwner);

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

        private void PrepareUserEntity(UserCommandBase command , User user)
        {
            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.BirthDate = command.BirthDate;
            user.Gender = command.Gender;
            user.AvatarId = command.AvatarId;
        }   
    }
}

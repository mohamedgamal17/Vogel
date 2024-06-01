using Vogel.Application.Medias.Policies;
using Vogel.Application.Users.Dtos;
using Vogel.Application.Users.Factories;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Application.Security;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Repositories;
using Vogel.BuildingBlocks.Domain.Results;
using Vogel.Domain.Medias;
using Vogel.Domain.Users;
namespace Vogel.Application.Users.Commands
{
    public class UserCommandHandler : 
        IApplicationRequestHandler<CreateUserCommand, UserDto>,
        IApplicationRequestHandler<UpdateUserCommand , UserDto>
    {
        private readonly IRepository<UserAggregate> _userRepository;
        private readonly IRepository<Media> _mediaRepository;
        private readonly IUserResponseFactory _userResponseFactory;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly ISecurityContext _securityContext;

        public UserCommandHandler(IRepository<UserAggregate> userRepository, 
            IRepository<Media> mediaRepository, 
            IUserResponseFactory userResponseFactory, 
            IApplicationAuthorizationService applicationAuthorizationService, 
            ISecurityContext securityContext)
        {
            _userRepository = userRepository;
            _mediaRepository = mediaRepository;
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

            var user = new UserAggregate(userId);

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

            user = await _userRepository.InsertAsync(user);

            return await _userResponseFactory.PrepareUserAggregateDto(user, avatar);
        }

        public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            string currentUserId = _securityContext.User!.Id!;

            var user = await _userRepository.FindByIdAsync(currentUserId);

            if(user == null)
            {
                return new Result<UserDto>(new EntityNotFoundException(typeof(UserAggregate), currentUserId));
            }

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

            await _userRepository.UpdateAsync(user);

            if(avatar == null  && user.AvatarId != null)
            {
                avatar = await _mediaRepository.FindByIdAsync(user.AvatarId);
            }

            return await _userResponseFactory.PrepareUserAggregateDto(user, avatar);
        }

        private void PrepareUserEntity(UserCommandBase command , UserAggregate user)
        {
            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.BirthDate = command.BirthDate;
            user.Gender = command.Gender;
            user.AvatarId = command.AvatarId;
        }   
    }
}

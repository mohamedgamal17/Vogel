using MongoDB.Driver;
using Vogel.Application.Common.Exceptions;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Medias.Policies;
using Vogel.Application.Users.Dtos;
using Vogel.Application.Users.Factories;
using Vogel.Domain.Medias;
using Vogel.Domain.Utils;

namespace Vogel.Application.Users.Commands
{
    public class UserCommandHandler : 
        IApplicationRequestHandler<CreateUserCommand, UserAggregateDto>,
        IApplicationRequestHandler<UpdateUserCommand , UserAggregateDto>
    {
        private readonly IMongoDbRepository<User> _userRepository;
        private readonly IMongoDbRepository<Media> _mediaRepository;
        private readonly IUserResponseFactory _userResponseFactory;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly ISecurityContext _securityContext;

        public UserCommandHandler(IMongoDbRepository<User> userRepository, IMongoDbRepository<Media> mediaRepository, IUserResponseFactory userResponseFactory, IApplicationAuthorizationService applicationAuthorizationService, ISecurityContext securityContext)
        {
            _userRepository = userRepository;
            _mediaRepository = mediaRepository;
            _userResponseFactory = userResponseFactory;
            _applicationAuthorizationService = applicationAuthorizationService;
            _securityContext = securityContext;
        }

        public async Task<Result<UserAggregateDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            string userId = _securityContext.User!.Id;

            bool isUserExist = (await _userRepository.FindByIdAsync(userId)) != null;

            if (isUserExist)
            {
                return new Result<UserAggregateDto>(new BusinessLogicException("User already has profile"));
            }

            var user = new User(userId);

            if (request.AvatarId != null)
            {
                var media = await _mediaRepository.SingleAsync(new FilterDefinitionBuilder<Media>().Eq(x => x.Id, request.AvatarId));

                var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(media, MediaOperationRequirements.IsOwner);

                if (authorizationResult.IsFailure)
                {
                    return new Result<UserAggregateDto>(authorizationResult.Exception!);
                }
            }

            PrepareUserEntity(request, user);

            user = await _userRepository.InsertAsync(user);

            return await _userResponseFactory.PrepareUserAggregateDto(user);
        }

        public async Task<Result<UserAggregateDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            string currentUserId = _securityContext.User!.Id!;

            var user = await _userRepository.FindByIdAsync(currentUserId);


            if(user == null)
            {
                return new Result<UserAggregateDto>(new EntityNotFoundException(typeof(User), currentUserId));
            }

            if (request.AvatarId != null)
            {
                var media = await _mediaRepository.SingleAsync(new FilterDefinitionBuilder<Media>().Eq(x => x.Id, request.AvatarId));

                var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(media, MediaOperationRequirements.IsOwner);

                if (authorizationResult.IsFailure)
                {
                    return new Result<UserAggregateDto>(authorizationResult.Exception!);
                }
            }

            PrepareUserEntity(request, user);

            await _userRepository.UpdateAsync(user);

            return await _userResponseFactory.PrepareUserAggregateDto(user);
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

using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Users.Factories;
using Vogel.Social.Domain.Users;
using Vogel.Social.MongoEntities.Users;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Users.Queries.GetCurrentUser
{
    public class GetCurrentUserQueryHandler : IApplicationRequestHandler<GetCurrentUserQuery, UserDto>
    {
        private readonly UserMongoRepository _userMongoRepository;
        private readonly IUserResponseFactory _userResponseFactory;
        private readonly ISecurityContext _securityContext;

        public GetCurrentUserQueryHandler(UserMongoRepository userMongoRepository, IUserResponseFactory userResponseFactory, ISecurityContext securityContext)
        {
            _userMongoRepository = userMongoRepository;
            _userResponseFactory = userResponseFactory;
            _securityContext = securityContext;
        }

        public async Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            string currentUserId = _securityContext.User!.Id;

            var user = await _userMongoRepository.GetByIdUserMongoView(currentUserId);

            if (user == null)
            {
                return new Result<UserDto>(new EntityNotFoundException(typeof(User), currentUserId));
            }

            return await _userResponseFactory.PrepareUserDto(user);
        }
    }
}

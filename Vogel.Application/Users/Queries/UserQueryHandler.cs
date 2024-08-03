using Vogel.Application.Users.Dtos;
using Vogel.Application.Users.Factories;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Application.Security;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Results;
using Vogel.Domain.Users;
using Vogel.MongoDb.Entities.Common;
using Vogel.MongoDb.Entities.Users;
namespace Vogel.Application.Users.Queries
{
    public class UserQueryHandler :
        IApplicationRequestHandler<ListUserQuery, Paging<UserDto>>,
        IApplicationRequestHandler<GetCurrentUserQuery, UserDto>,
        IApplicationRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserResponseFactory _userResponseFactory;
        private readonly ISecurityContext _securityContext;
        private readonly UserMongoRepository _userMongoRepository;

        public UserQueryHandler(IUserResponseFactory userResponseFactory, ISecurityContext securityContext, UserMongoRepository userMongoRepository)
        {
            _userResponseFactory = userResponseFactory;
            _securityContext = securityContext;
            _userMongoRepository = userMongoRepository;
        }

        public async Task<Result<Paging<UserDto>>> Handle(ListUserQuery request, CancellationToken cancellationToken)
        {
            var paged = await _userMongoRepository.GetUserViewPaged(request.Cursor, request.Asending, request.Limit);

            var result = new Paging<UserDto>
            {
                Data = await _userResponseFactory.PrepareListUserDto(paged.Data),
                Info = paged.Info
            };

            return result;
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

        public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userMongoRepository.GetByIdUserMongoView(request.Id);

            if (user == null)
            {
                return new Result<UserDto>(new EntityNotFoundException(typeof(User), request.Id));
            }

            return await _userResponseFactory.PrepareUserDto(user);
        }
    }
}

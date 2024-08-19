using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Users.Factories;
using Vogel.Social.Domain.Users;
using Vogel.Social.MongoEntities.Users;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Users.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IApplicationRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly UserMongoRepository _userMongoRepository;
        private readonly IUserResponseFactory _userResponseFactory;

        public GetUserByIdQueryHandler(UserMongoRepository userMongoRepository, IUserResponseFactory userResponseFactory)
        {
            _userMongoRepository = userMongoRepository;
            _userResponseFactory = userResponseFactory;
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

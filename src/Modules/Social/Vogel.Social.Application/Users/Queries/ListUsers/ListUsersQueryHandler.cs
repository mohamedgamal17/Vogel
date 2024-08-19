using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Users.Factories;
using Vogel.Social.MongoEntities.Users;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Users.Queries.ListUsers
{
    public class ListUsersQueryHandler : IApplicationRequestHandler<ListUsersQuery, Paging<UserDto>>
    {
        private readonly UserMongoRepository _userMongoRepository;
        private readonly IUserResponseFactory _userResponseFactory;

        public ListUsersQueryHandler(UserMongoRepository userMongoRepository, IUserResponseFactory userResponseFactory)
        {
            _userMongoRepository = userMongoRepository;
            _userResponseFactory = userResponseFactory;
        }

        public async Task<Result<Paging<UserDto>>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
        {
            var paged = await _userMongoRepository.GetUserViewPaged(request.Cursor, request.Asending, request.Limit);

            var result = new Paging<UserDto>
            {
                Data = await _userResponseFactory.PrepareListUserDto(paged.Data),
                Info = paged.Info
            };

            return result;
        }
    }
}

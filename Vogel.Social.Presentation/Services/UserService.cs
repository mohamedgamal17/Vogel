using MediatR;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Users.Queries.GetUserById;
using Vogel.Social.Application.Users.Queries.ListUsers;
using Vogel.Social.Shared.Dtos;
using Vogel.Social.Shared.Services;

namespace Vogel.Social.Presentation.Services
{
    public class UserService : IUserService
    {
        private readonly IMediator _mediator;

        public UserService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result<UserDto>> GetUserById(string id)
        {
            return await _mediator.Send(new GetUserByIdQuery { Id = id });
        }

        public async Task<Result<Paging<UserDto>>> ListUsers(string? cursor = null, bool ascending = false, int limit = 10)
        {
            return await _mediator.Send(new ListUsersQuery
            {
                Cursor = cursor,
                Asending = ascending,
                Limit = limit
            });
        }
    }
}

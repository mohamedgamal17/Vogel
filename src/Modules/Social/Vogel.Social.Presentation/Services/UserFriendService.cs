using MediatR;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Friendship.Queries.ListFriends;
using Vogel.Social.Shared.Dtos;
using Vogel.Social.Shared.Services;

namespace Vogel.Social.Presentation.Services
{
    public class UserFriendService : IUserFriendService
    {
        private readonly IMediator _mediator;

        public UserFriendService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result<Paging<FriendDto>>> ListFriends(string userId ,string? cursor = null, bool ascending = false, int limit = 10)
        {
            var query = new ListFriendsQuery
            {
                UserId = userId,
                Cursor = cursor,
                Asending = ascending,
                Limit = limit
            };

            var result = await _mediator.Send(query);

            return result;
        }
    }
}

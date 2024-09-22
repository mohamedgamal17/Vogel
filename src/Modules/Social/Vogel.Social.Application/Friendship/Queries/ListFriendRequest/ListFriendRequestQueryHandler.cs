using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Friendship.Factories;
using Vogel.Social.MongoEntities.Friendship;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Friendship.Queries.ListFriendRequest
{
    public class ListFriendRequestQueryHandler : IApplicationRequestHandler<ListFriendRequestQuery, Paging<FriendRequestDto>>
    {
        private readonly FriendRequestMongoRepository _friendRequestMongoRepository;
        private readonly IFriendshipResponseFactory _friendshipResponseFactory;

        public ListFriendRequestQueryHandler(FriendRequestMongoRepository friendRequestMongoRepository, IFriendshipResponseFactory friendshipResponseFactory)
        {
            _friendRequestMongoRepository = friendRequestMongoRepository;
            _friendshipResponseFactory = friendshipResponseFactory;
        }

        public async Task<Result<Paging<FriendRequestDto>>> Handle(ListFriendRequestQuery request, CancellationToken cancellationToken)
        {
            var paged = await _friendRequestMongoRepository.GetFriendRequestViewPaged(request.UserId, request.Cursor,
                    request.Limit, request.Asending);

            var response = new Paging<FriendRequestDto>()
            {
                Data = await _friendshipResponseFactory.PrepareListFriendRequestDto(paged.Data),
                Info = paged.Info
            };

            return response;
        }
    }
}

using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Friendship.Factories;
using Vogel.Social.MongoEntities.Friendship;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Friendship.Queries.ListFriends
{
    public class ListFriendsQueryHandler : IApplicationRequestHandler<ListFriendsQuery, Paging<FriendDto>>
    {
        private readonly FriendMongoRepository _friendMongoRepository;
        private readonly IFriendshipResponseFactory _friendResponseFactory;

        public ListFriendsQueryHandler(FriendMongoRepository friendMongoRepository, IFriendshipResponseFactory friendResponseFactory)
        {
            _friendMongoRepository = friendMongoRepository;
            _friendResponseFactory = friendResponseFactory;
        }

        public async Task<Result<Paging<FriendDto>>> Handle(ListFriendsQuery request, CancellationToken cancellationToken)
        {
            var paged = await _friendMongoRepository.GetFriendViewPaged(request.UserId, request.Cursor, request.Limit, request.Asending);

            var response = new Paging<FriendDto>
            {
                Data = await _friendResponseFactory.PrepareListFriendDto(paged.Data),
                Info = paged.Info
            };

            return response;
        }
    }
}

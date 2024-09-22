using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Friendship.Factories;
using Vogel.Social.Application.Friendship.Policies;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.MongoEntities.Friendship;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Friendship.Queries.GetFriendRequestById
{
    public class GetFriendRequestByIdQueryHandler : IApplicationRequestHandler<GetFriendRequestByIdQuery, FriendRequestDto>
    {
        private readonly FriendRequestMongoRepository _friendRequestMongoRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly IFriendshipResponseFactory _friendshipResponseFactory;

        public GetFriendRequestByIdQueryHandler(FriendRequestMongoRepository friendRequestMongoRepository, IApplicationAuthorizationService applicationAuthorizationService, IFriendshipResponseFactory friendshipResponseFactory)
        {
            _friendRequestMongoRepository = friendRequestMongoRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
            _friendshipResponseFactory = friendshipResponseFactory;
        }

        public async Task<Result<FriendRequestDto>> Handle(GetFriendRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var friendRequest = await _friendRequestMongoRepository.GetFriendRequestViewbyId(request.FriendRequestId);

            if (friendRequest == null)
            {
                return new Result<FriendRequestDto>(new EntityNotFoundException(typeof(FriendRequest), request.FriendRequestId));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(friendRequest, FriendshipOperationalRequirments.IsFriendRequestOwner);

            if (authorizationResult.IsFailure)
            {
                return new Result<FriendRequestDto>(authorizationResult.Exception!);
            }

            return await _friendshipResponseFactory.PrepareFriendRequestDto(friendRequest);
        }
    }
}

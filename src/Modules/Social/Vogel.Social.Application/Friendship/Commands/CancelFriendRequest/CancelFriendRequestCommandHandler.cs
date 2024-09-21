using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Friendship.Factories;
using Vogel.Social.Application.Friendship.Policies;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.MongoEntities.Friendship;
using Vogel.Social.Shared.Common;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Friendship.Commands.CancelFriendRequest
{
    public class CancelFriendRequestCommandHandler : IApplicationRequestHandler<CancelFriendRequestCommand, FriendRequestDto>
    {
        private readonly ISocialRepository<FriendRequest> _friendRequestRepository;
        private readonly FriendRequestMongoRepository _friendRequestMongoRepository;
        private readonly IFriendshipResponseFactory _friendshipResponseFactory;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public CancelFriendRequestCommandHandler(ISocialRepository<FriendRequest> friendRequestRepository, FriendRequestMongoRepository friendRequestMongoRepository, IFriendshipResponseFactory friendshipResponseFactory, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _friendRequestRepository = friendRequestRepository;
            _friendRequestMongoRepository = friendRequestMongoRepository;
            _friendshipResponseFactory = friendshipResponseFactory;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<FriendRequestDto>> Handle(CancelFriendRequestCommand request, CancellationToken cancellationToken)
        {
            var friendRequest = await _friendRequestRepository.FindByIdAsync(request.FriendRequestId);

            if (friendRequest == null)
            {
                return new Result<FriendRequestDto>(new EntityNotFoundException(typeof(FriendRequest), request.FriendRequestId));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(friendRequest
                , FriendshipOperationalRequirments.FriendRequestSenderAction);

            if (authorizationResult.IsFailure)
            {
                return new Result<FriendRequestDto>(authorizationResult.Exception!);
            }

            if (friendRequest.State != FriendRequestState.Pending)
            {
                return new Result<FriendRequestDto>(new BusinessLogicException($"Cannot cancel this friend request with id : {friendRequest.Id}"));
            }
            friendRequest.Cancel();

            await _friendRequestRepository.UpdateAsync(friendRequest);

            var friendRequestView = await _friendRequestMongoRepository
                .GetFriendRequestViewbyId(friendRequest.Id);

            return await _friendshipResponseFactory.PrepareFriendRequestDto(friendRequestView!);
        }
    }
}

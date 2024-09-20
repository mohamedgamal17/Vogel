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
namespace Vogel.Social.Application.Friendship.Commands.RejectFriendRequest
{
    public class RejectFriendRequestCommandHandler : IApplicationRequestHandler<RejectFriendRequestCommand, FriendRequestDto>
    {
        private readonly ISocialRepository<FriendRequest> _friendRequestRepository;
        private readonly FriendRequestMongoRepository _friendRequestMongoRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly IFriendshipResponseFactory _friendshipResponseFactory;

        public RejectFriendRequestCommandHandler(ISocialRepository<FriendRequest> friendRequestRepository, FriendRequestMongoRepository friendRequestMongoRepository, IApplicationAuthorizationService applicationAuthorizationService, IFriendshipResponseFactory friendshipResponseFactory)
        {
            _friendRequestRepository = friendRequestRepository;
            _friendRequestMongoRepository = friendRequestMongoRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
            _friendshipResponseFactory = friendshipResponseFactory;
        }

        public async Task<Result<FriendRequestDto>> Handle(RejectFriendRequestCommand request, CancellationToken cancellationToken)
        {
            var friendRequest = await _friendRequestRepository.FindByIdAsync(request.FriendRequestId);

            if (friendRequest == null)
            {
                return new Result<FriendRequestDto>(new EntityNotFoundException(typeof(FriendRequest), request.FriendRequestId));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(friendRequest
                , FriendshipOperationalRequirments.FriendRequestReciverAction);

            if (authorizationResult.IsFailure)
            {
                return new Result<FriendRequestDto>(authorizationResult.Exception!);
            }

            if (friendRequest.State != FriendRequestState.Pending)
            {
                return new Result<FriendRequestDto>(new BusinessLogicException($"Cannot Reject this friend request with id : {friendRequest.Id}"));
            }

            friendRequest.Reject();

            await _friendRequestRepository.UpdateAsync(friendRequest);

            var friendRequestView = await _friendRequestMongoRepository.GetFriendRequestViewbyId(friendRequest.Id);

            return await _friendshipResponseFactory.PrepareFriendRequestDto(friendRequestView!);
        }
    }
}

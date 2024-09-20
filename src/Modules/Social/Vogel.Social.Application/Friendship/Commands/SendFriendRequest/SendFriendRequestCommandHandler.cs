using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Friendship.Factories;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.MongoEntities.Friendship;
using Vogel.Social.Shared.Common;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Friendship.Commands.SendFriendRequest
{
    public class SendFriendRequestCommandHandler : IApplicationRequestHandler<SendFriendRequestCommand, FriendRequestDto>
    {
        private readonly ISocialRepository<FriendRequest> _friendRequestRepository;
        private readonly FriendRequestMongoRepository _friendRequestMongoRepository;
        private readonly ISecurityContext _securityContext;
        private readonly IFriendshipResponseFactory _friendshipResponseFactory;

        public SendFriendRequestCommandHandler(ISocialRepository<FriendRequest> friendRequestRepository, FriendRequestMongoRepository friendRequestMongoRepository, ISecurityContext securityContext, IFriendshipResponseFactory friendshipResponseFactory)
        {
            _friendRequestRepository = friendRequestRepository;
            _friendRequestMongoRepository = friendRequestMongoRepository;
            _securityContext = securityContext;
            _friendshipResponseFactory = friendshipResponseFactory;
        }

        public async Task<Result<FriendRequestDto>> Handle(SendFriendRequestCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _securityContext.User!.Id;

            bool pendingRequests = await IsThereIsAnyPendingRequest(currentUserId, request.ReciverId);

            if (pendingRequests)
            {
                return new Result<FriendRequestDto>(
                    new BusinessLogicException("There is already friend request sent to that user and action is not taken on it"));
            }

            var friendRequest = new FriendRequest
            {
                SenderId = currentUserId,
                ReciverId = request.ReciverId
            };

            await _friendRequestRepository.InsertAsync(friendRequest);

            var friendRequestView = await _friendRequestMongoRepository
                .GetFriendRequestViewbyId(friendRequest.Id);

            return await _friendshipResponseFactory.PrepareFriendRequestDto(friendRequestView!);
        }


        private async Task<bool> IsThereIsAnyPendingRequest(string senderId, string reciverId)
        {
            return await _friendRequestRepository.AnyAsync(x => x.SenderId == senderId
            && x.ReciverId == reciverId
            && x.State == FriendRequestState.Pending);
        }
    }
}

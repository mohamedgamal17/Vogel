using MediatR;
using Vogel.Application.Friendship.Dtos;
using Vogel.Application.Friendship.Factories;
using Vogel.Application.Friendship.Policies;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Repositories;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Domain.Friendship;
using Vogel.MongoDb.Entities.Friendship;
namespace Vogel.Application.Friendship.Commands
{
    public class FriendshipCommandHandler :
        IApplicationRequestHandler<SendFriendRequestCommand, FriendRequestDto>,
        IApplicationRequestHandler<AcceptFriendRequestCommand, FriendDto>,
        IApplicationRequestHandler<RejectFriendRequestCommand, FriendRequestDto>,
        IApplicationRequestHandler<CancelFriendRequestCommand , FriendRequestDto>,
        IApplicationRequestHandler<RemoveFriendCommand,Unit>
    {
        private readonly IRepository<FriendRequest> _friendRequestRepository;
        private readonly IRepository<Friend> _friendRepository;
        private readonly FriendRequestMongoRepository _friendRequestMongoRepository;
        private readonly FriendMongoRepository _friendMongoRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly ISecurityContext _securityContext;
        private readonly IFriendshipResponseFactory _friendshipResponseFactory;

        public FriendshipCommandHandler(IRepository<FriendRequest> friendRequestRepository, IRepository<Friend> friendRepository, FriendRequestMongoRepository friendRequestMongoRepository, FriendMongoRepository friendMongoRepository, IApplicationAuthorizationService applicationAuthorizationService, ISecurityContext securityContext, IFriendshipResponseFactory friendshipResponseFactory)
        {
            _friendRequestRepository = friendRequestRepository;
            _friendRepository = friendRepository;
            _friendRequestMongoRepository = friendRequestMongoRepository;
            _friendMongoRepository = friendMongoRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
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

        public async Task<Result<FriendDto>> Handle(AcceptFriendRequestCommand request, CancellationToken cancellationToken)
        {
            var friendRequest = await _friendRequestRepository.FindByIdAsync(request.FriendRequestId);

            if(friendRequest == null)
            {
                return new Result<FriendDto>(new EntityNotFoundException(typeof(FriendRequest), request.FriendRequestId));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(friendRequest
                , FriendshipOperationalRequirments.FriendRequestReciverAction);

            if (authorizationResult.IsFailure)
            {
                return new Result<FriendDto>(authorizationResult.Exception!);
            }

            if(friendRequest.State != Domain.Friendship.FriendRequestState.Pending)
            {
                return new Result<FriendDto>(new BusinessLogicException($"Cannot accept this friend request with id : {friendRequest.Id}"));
            }


            friendRequest.Accept();

            var friend = new Friend
            {
                SourceId = friendRequest.SenderId,
                TargetId = friendRequest.ReciverId,
            };

            await _friendRequestRepository.UpdateAsync(friendRequest);

            await _friendRepository.InsertAsync(friend);

            var friendRequestView = await _friendMongoRepository.GetFriendViewbyId(friend.Id);

            return await _friendshipResponseFactory.PrepareFriendDto(friendRequestView!);

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

            if (friendRequest.State != Domain.Friendship.FriendRequestState.Pending)
            {
                return new Result<FriendRequestDto>(new BusinessLogicException($"Cannot Reject this friend request with id : {friendRequest.Id}"));
            }

            friendRequest.Reject();

            await _friendRequestRepository.UpdateAsync(friendRequest);

            var friendRequestView = await _friendRequestMongoRepository.GetFriendRequestViewbyId(friendRequest.Id);

            return await _friendshipResponseFactory.PrepareFriendRequestDto(friendRequestView!);

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

            if (friendRequest.State != Domain.Friendship.FriendRequestState.Pending)
            {
                return new Result<FriendRequestDto>(new BusinessLogicException($"Cannot cancel this friend request with id : {friendRequest.Id}"));
            }

            friendRequest.Cancel();

            await _friendRequestRepository.UpdateAsync(friendRequest);

            var friendRequestView = await _friendRequestMongoRepository
                .GetFriendRequestViewbyId(friendRequest.Id);

            return await _friendshipResponseFactory.PrepareFriendRequestDto(friendRequestView!);

        }

        public async Task<Result<Unit>> Handle(RemoveFriendCommand request, CancellationToken cancellationToken)
        {
            var friend = await _friendRepository.FindByIdAsync(request.Id);

            if(friend == null)
            {
                return new Result<Unit>(new EntityNotFoundException(typeof(Friend), request.Id));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(friend
            , FriendshipOperationalRequirments.IsFriendOwner);

            if (authorizationResult.IsFailure)
            {
                return new Result<Unit>(authorizationResult.Exception!);
            }

            await _friendRepository.DeleteAsync(friend);

            return Unit.Value;
        }

        private async Task<bool> IsThereIsAnyPendingRequest(string senderId, string reciverId)
        {
            return await _friendRequestRepository.AnyAsync(x => x.SenderId == senderId
            && x.ReciverId == reciverId
            && x.State == Domain.Friendship.FriendRequestState.Pending);
        }
    }
}

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

namespace Vogel.Social.Application.Friendship.Commands.AcceptFriendRequest
{
    public class AcceptFriendRequestCommandHandler : IApplicationRequestHandler<AcceptFriendRequestCommand, FriendDto>
    {
        private readonly ISocialRepository<FriendRequest> _friendRequestRepository;
        private readonly ISocialRepository<Friend> _friendRepository;
        private readonly FriendMongoRepository _friendMongoRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly IFriendshipResponseFactory _friendshipResponseFactory;

        public AcceptFriendRequestCommandHandler(ISocialRepository<FriendRequest> friendRequestRepository, ISocialRepository<Friend> friendRepository, FriendMongoRepository friendMongoRepository, IApplicationAuthorizationService applicationAuthorizationService, IFriendshipResponseFactory friendshipResponseFactory)
        {
            _friendRequestRepository = friendRequestRepository;
            _friendRepository = friendRepository;
            _friendMongoRepository = friendMongoRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
            _friendshipResponseFactory = friendshipResponseFactory;
        }

        public async Task<Result<FriendDto>> Handle(AcceptFriendRequestCommand request, CancellationToken cancellationToken)
        {
            var friendRequest = await _friendRequestRepository.FindByIdAsync(request.FriendRequestId);

            if (friendRequest == null)
            {
                return new Result<FriendDto>(new EntityNotFoundException(typeof(FriendRequest), request.FriendRequestId));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(friendRequest
                , FriendshipOperationalRequirments.FriendRequestReciverAction);

            if (authorizationResult.IsFailure)
            {
                return new Result<FriendDto>(authorizationResult.Exception!);
            }

            if (friendRequest.State != FriendRequestState.Pending)
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
    }
}

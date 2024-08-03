using MongoDB.Driver;
using Vogel.Application.Friendship.Dtos;
using Vogel.Application.Friendship.Factories;
using Vogel.Application.Friendship.Policies;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Application.Security;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Domain.Results;
using Vogel.Domain.Friendship;
using Vogel.MongoDb.Entities.Common;
using Vogel.MongoDb.Entities.Friendship;
namespace Vogel.Application.Friendship.Queries
{
    public class FriendRequestQueryHandler : 
        IApplicationRequestHandler<ListFriendRequestQuery, Paging<FriendRequestDto>>,
        IApplicationRequestHandler<GetFriendRequestByIdQuery, FriendRequestDto>
    {
        private readonly FriendRequestMongoRepository _friendRequestMongoRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly IFriendshipResponseFactory _friendshipResponseFactory;

        public FriendRequestQueryHandler(FriendRequestMongoRepository friendRequestMongoRepository, IApplicationAuthorizationService applicationAuthorizationService, IFriendshipResponseFactory friendshipResponseFactory)
        {
            _friendRequestMongoRepository = friendRequestMongoRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
            _friendshipResponseFactory = friendshipResponseFactory;
        }

        public async Task<Result<Paging<FriendRequestDto>>> Handle(ListFriendRequestQuery request, CancellationToken cancellationToken)
        {

            var paged = await _friendRequestMongoRepository.GetFriendRequestViewPaged(request.UserId, request.Cursor, 
                request.Limit , request.Asending);

            var response = new Paging<FriendRequestDto>()
            {
                Data = await _friendshipResponseFactory.PrepareListFriendRequestDto(paged.Data),
                Info = paged.Info
            };

            return response;
        }

        public async Task<Result<FriendRequestDto>> Handle(GetFriendRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var friendRequest = await _friendRequestMongoRepository.GetFriendRequestViewbyId(request.Id);

            if(friendRequest == null)
            {
                return new Result<FriendRequestDto>(new EntityNotFoundException(typeof(FriendRequest), request.Id));
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

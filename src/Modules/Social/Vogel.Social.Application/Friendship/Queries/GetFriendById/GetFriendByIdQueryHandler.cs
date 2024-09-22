using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Friendship.Factories;
using Vogel.Social.Application.Friendship.Policies;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.MongoEntities.Friendship;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Friendship.Queries.GetFriendById
{
    public class GetFriendByIdQueryHandler : IApplicationRequestHandler<GetFriendByIdQuery, FriendDto>
    {
        private readonly FriendMongoRepository _friendMongoRepository;
        private readonly IFriendshipResponseFactory _friendResponseFactory;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public GetFriendByIdQueryHandler(FriendMongoRepository friendMongoRepository, IFriendshipResponseFactory friendResponseFactory, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _friendMongoRepository = friendMongoRepository;
            _friendResponseFactory = friendResponseFactory;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<FriendDto>> Handle(GetFriendByIdQuery request, CancellationToken cancellationToken)
        {
            var friend = await _friendMongoRepository.GetFriendViewbyId(request.FriendId);

            if (friend == null)
            {
                return new Result<FriendDto>(new EntityNotFoundException(typeof(Friend), request.FriendId));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(friend, FriendshipOperationalRequirments.IsFriendOwner);

            if (authorizationResult.IsFailure)
            {
                return new Result<FriendDto>(authorizationResult.Exception!);
            }

            return await _friendResponseFactory.PrepareFriendDto(friend);
        }
    }
}

using Vogel.Application.Friendship.Dtos;
using Vogel.Application.Friendship.Factories;
using Vogel.Application.Friendship.Policies;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Domain.Friendship;
using Vogel.MongoDb.Entities.Common;
using Vogel.MongoDb.Entities.Friendship;
namespace Vogel.Application.Friendship.Queries
{
    public class FriendQueryHandler :
        IApplicationRequestHandler<ListFriendQuery, Paging<FriendDto>>,
        IApplicationRequestHandler<GetFriendByIdQuery, FriendDto>
    {
        private readonly FriendMongoRepository _friendMongoRepository;
        private readonly IFriendshipResponseFactory _friendResponseFactory;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public FriendQueryHandler(FriendMongoRepository friendMongoRepository, IFriendshipResponseFactory friendResponseFactory, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _friendMongoRepository = friendMongoRepository;
            _friendResponseFactory = friendResponseFactory;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<Paging<FriendDto>>> Handle(ListFriendQuery request, CancellationToken cancellationToken)
        {
            var paged = await _friendMongoRepository.GetFriendViewPaged(request.UserId, request.Cursor, request.Limit, request.Asending);

            var response = new Paging<FriendDto>
            {
                Data = await _friendResponseFactory.PrepareListFriendDto(paged.Data),
                Info = paged.Info
            };

            return response;
        }

        public async Task<Result<FriendDto>> Handle(GetFriendByIdQuery request, CancellationToken cancellationToken)
        {
            var friend = await _friendMongoRepository.GetFriendViewbyId(request.Id);

            if(friend == null)
            {
                return new Result<FriendDto>(new EntityNotFoundException(typeof(Friend), request.Id));
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

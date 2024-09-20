using MediatR;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Social.Application.Friendship.Policies;
using Vogel.Social.Domain;
using Vogel.Social.Domain.Friendship;

namespace Vogel.Social.Application.Friendship.Commands.RemoveFriend
{
    public class RemoveFriendCommandHandler : IApplicationRequestHandler<RemoveFriendCommand, Unit>
    {
        private readonly ISocialRepository<Friend> _friendRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public RemoveFriendCommandHandler(ISocialRepository<Friend> friendRepository, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _friendRepository = friendRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<Unit>> Handle(RemoveFriendCommand request, CancellationToken cancellationToken)
        {
            var friend = await _friendRepository.FindByIdAsync(request.Id);

            if (friend == null)
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
    }
}

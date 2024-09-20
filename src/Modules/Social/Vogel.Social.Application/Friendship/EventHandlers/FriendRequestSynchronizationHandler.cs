using AutoMapper;
using MediatR;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.MongoEntities.Friendship;
namespace Vogel.Social.Application.Friendship.EventHandlers
{
    public class FriendRequestSynchronizationHandler :
        INotificationHandler<EntityCreatedEvent<FriendRequest>>,
        INotificationHandler<EntityUpdatedEvent<FriendRequest>>,
        INotificationHandler<EntityDeletedEvent<FriendRequest>>
    {

        private readonly IMapper _mapper;

        private readonly FriendRequestMongoRepository _friendRequestMongoRepository;

        public FriendRequestSynchronizationHandler(IMapper mapper, FriendRequestMongoRepository friendRequestMongoRepository)
        {
            _mapper = mapper;
            _friendRequestMongoRepository = friendRequestMongoRepository;
        }

        public async Task Handle(EntityCreatedEvent<FriendRequest> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<FriendRequest, FriendRequestMongoEntity>(notification.Entity);

            await _friendRequestMongoRepository.InsertAsync(mongoEntity);
        }

        public async Task Handle(EntityUpdatedEvent<FriendRequest> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<FriendRequest, FriendRequestMongoEntity>(notification.Entity);

            await _friendRequestMongoRepository.UpdateAsync(mongoEntity);

        }

        public async Task Handle(EntityDeletedEvent<FriendRequest> notification, CancellationToken cancellationToken)
        {
            await _friendRequestMongoRepository.DeleteAsync(notification.Entity.Id);
        }
    }
}

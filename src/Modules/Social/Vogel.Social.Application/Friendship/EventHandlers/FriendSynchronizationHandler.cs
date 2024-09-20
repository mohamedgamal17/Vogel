using AutoMapper;
using MediatR;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.MongoEntities.Friendship;
namespace Vogel.Social.Application.Friendship.EventHandlers
{
    public class FriendSynchronizationHandler :
        INotificationHandler<EntityCreatedEvent<Friend>>,
        INotificationHandler<EntityUpdatedEvent<Friend>>,
        INotificationHandler<EntityDeletedEvent<Friend>>
    {
        private readonly IMapper _mapper;
        private readonly FriendMongoRepository _friendMongoRepository;

        public FriendSynchronizationHandler(IMapper mapper, FriendMongoRepository friendMongoRepository)
        {
            _mapper = mapper;
            _friendMongoRepository = friendMongoRepository;
        }

        public async Task Handle(EntityCreatedEvent<Friend> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<Friend, FriendMongoEntity>(notification.Entity);
            await _friendMongoRepository.InsertAsync(mongoEntity);
        }

        public async Task Handle(EntityUpdatedEvent<Friend> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<Friend, FriendMongoEntity>(notification.Entity);

            await _friendMongoRepository.UpdateAsync(mongoEntity);
        }

        public async Task Handle(EntityDeletedEvent<Friend> notification, CancellationToken cancellationToken)
        {
            await _friendMongoRepository.DeleteAsync(notification.Entity.Id);
        }
    }
}

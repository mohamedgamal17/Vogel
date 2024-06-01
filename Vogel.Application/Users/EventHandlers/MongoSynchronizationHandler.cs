using AutoMapper;
using MediatR;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.Domain.Users;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.Application.Users.EventHandlers
{
    public class MongoSynchronizationHandler :
        INotificationHandler<EntityCreatedEvent<UserAggregate>>,
        INotificationHandler<EntityUpdatedEvent<UserAggregate>>,
        INotificationHandler<EntityDeletedEvent<UserAggregate>>
    {
        private readonly UserMongoRepository _userMongoRepository;

        private readonly IMapper _mapper;

        public MongoSynchronizationHandler(UserMongoRepository userMongoRepository, IMapper mapper)
        {
            _userMongoRepository = userMongoRepository;
            _mapper = mapper;
        }

        public async Task Handle(EntityCreatedEvent<UserAggregate> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<UserAggregate, UserMongoEntity>(notification.Entity);

            await _userMongoRepository.InsertAsync(mongoEntity);
        }

        public async Task Handle(EntityUpdatedEvent<UserAggregate> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<UserAggregate, UserMongoEntity>(notification.Entity);

            await _userMongoRepository.UpdateAsync(mongoEntity);
        }

        public async Task Handle(EntityDeletedEvent<UserAggregate> notification, CancellationToken cancellationToken)
        {
            await _userMongoRepository.DeleteAsync(notification.Entity.Id);
        }
    }
}

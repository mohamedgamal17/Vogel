using AutoMapper;
using MediatR;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.Social.Domain.Users;
using Vogel.Social.MongoEntities.Users;

namespace Vogel.Social.Application.Users.Handlers
{
    public class MongoSynchronizationHandler :
        INotificationHandler<EntityCreatedEvent<User>>,
        INotificationHandler<EntityUpdatedEvent<User>>,
        INotificationHandler<EntityDeletedEvent<User>>
    {
        private readonly UserMongoRepository _userMongoRepository;

        private readonly IMapper _mapper;

        public MongoSynchronizationHandler(UserMongoRepository userMongoRepository, IMapper mapper)
        {
            _userMongoRepository = userMongoRepository;
            _mapper = mapper;
        }

        public async Task Handle(EntityCreatedEvent<User> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<User, UserMongoEntity>(notification.Entity);

            await _userMongoRepository.ReplaceOrInsertAsync(mongoEntity);
        }

        public async Task Handle(EntityUpdatedEvent<User> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<User, UserMongoEntity>(notification.Entity);

            await _userMongoRepository.ReplaceOrInsertAsync(mongoEntity);
        }

        public async Task Handle(EntityDeletedEvent<User> notification, CancellationToken cancellationToken)
        {
            await _userMongoRepository.DeleteAsync(notification.Entity.Id);
        }
    }
}

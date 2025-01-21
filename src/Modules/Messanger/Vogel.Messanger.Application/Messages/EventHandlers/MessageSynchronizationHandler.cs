using AutoMapper;
using MediatR;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Messanger.Domain.Messages;
using Vogel.Messanger.MongoEntities.Messages;

namespace Vogel.Messanger.Application.Messages.EventHandlers
{
    public class MessageSynchronizationHandler : 
        INotificationHandler<EntityCreatedEvent<Message>>,
        INotificationHandler<EntityUpdatedEvent<Message>>,
        INotificationHandler<EntityDeletedEvent<Message>>
    {
        private readonly IMapper _mapper;
        private readonly IMongoRepository<MessageMongoEntity> _messageMongoRepository;

        public MessageSynchronizationHandler(IMapper mapper, IMongoRepository<MessageMongoEntity> messageMongoRepository)
        {
            _mapper = mapper;
            _messageMongoRepository = messageMongoRepository;
        }

        public async Task Handle(EntityCreatedEvent<Message> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<Message, MessageMongoEntity>(notification.Entity);

            await _messageMongoRepository.ReplaceOrInsertAsync(mongoEntity);
        }

        public async Task Handle(EntityUpdatedEvent<Message> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<Message, MessageMongoEntity>(notification.Entity);
            await _messageMongoRepository.ReplaceOrInsertAsync(mongoEntity);
        }

        public async Task Handle(EntityDeletedEvent<Message> notification, CancellationToken cancellationToken)
        {
            await _messageMongoRepository.DeleteAsync(notification.Entity.Id); 
        }
    }

}

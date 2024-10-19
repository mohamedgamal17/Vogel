using AutoMapper;
using MediatR;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.MongoEntities.Conversations;
namespace Vogel.Messanger.Application.Conversations.Handlers
{
    public class ConversationSynchronizationHandler :
        INotificationHandler<EntityCreatedEvent<Conversation>>,
        INotificationHandler<EntityUpdatedEvent<Conversation>>
    {
        private readonly IMongoRepository<ConversationMongoEntity> _conversationMongoRepository;
        private readonly IMapper _mapper;
        public ConversationSynchronizationHandler(IMongoRepository<ConversationMongoEntity> conversationMongoRepository, IMapper mapper)
        {
            _conversationMongoRepository = conversationMongoRepository;
            _mapper = mapper;
        }

        public async Task Handle(EntityCreatedEvent<Conversation> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<Conversation, ConversationMongoEntity>(notification.Entity);

            await _conversationMongoRepository.InsertAsync(mongoEntity);
        }

        public async Task Handle(EntityUpdatedEvent<Conversation> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<Conversation, ConversationMongoEntity>(notification.Entity);

            await _conversationMongoRepository.UpdateAsync(mongoEntity);
        }
    }
}

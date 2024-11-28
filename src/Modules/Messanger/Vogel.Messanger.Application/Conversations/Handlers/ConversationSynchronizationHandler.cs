using AutoMapper;
using MediatR;
using MongoDB.Driver;
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

            await _conversationMongoRepository.ReplaceOrInsertAsync(mongoEntity);
        }

        public async Task Handle(EntityUpdatedEvent<Conversation> notification, CancellationToken cancellationToken)
        {

            var update = Builders<ConversationMongoEntity>.Update
                .Set(x => x.Name, notification.Entity.Name)
                .Set(x => x.CreationTime, notification.Entity.CreationTime)
                .Set(x => x.CreatorId, notification.Entity.CreatorId)
                .Set(x => x.ModificationTime, notification.Entity.ModificationTime)
                .Set(x => x.ModifierId, notification.Entity.ModifierId)
                .Set(x => x.DeletorId, notification.Entity.DeletorId)
                .Set(x => x.DeletionTime, notification.Entity.DeletionTime);

            await _conversationMongoRepository.UpdateAsync(notification.Entity.Id,update);
        }
    }
}

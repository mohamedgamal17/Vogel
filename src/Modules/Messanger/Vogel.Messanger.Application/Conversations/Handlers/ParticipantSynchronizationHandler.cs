using AutoMapper;
using MediatR;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.MongoEntities.Conversations;
namespace Vogel.Messanger.Application.Conversations.Handlers
{
    public class ParticipantSynchronizationHandler :
        INotificationHandler<EntityCreatedEvent<Participant>>,
        INotificationHandler<EntityUpdatedEvent<Participant>>
    {
        private readonly ConversationMongoRepository _conversationMongoRepository;
        private readonly IMapper _mapper;

        public ParticipantSynchronizationHandler(ConversationMongoRepository conversationMongoEntity, IMapper mapper)
        {
            _conversationMongoRepository = conversationMongoEntity;
            _mapper = mapper;
        }

        public async Task Handle(EntityCreatedEvent<Participant> notification, CancellationToken cancellationToken)
        {
            var participant = _mapper.Map<Participant, ParticipantMongoEntity>(notification.Entity);

            await _conversationMongoRepository.InsertParticipantAsync(notification.Entity.ConversationId, participant);
        }

        public async Task Handle(EntityUpdatedEvent<Participant> notification, CancellationToken cancellationToken)
        {
            var participant = _mapper.Map<Participant, ParticipantMongoEntity>(notification.Entity);

            await _conversationMongoRepository.UpdateParticipantAsync(notification.Entity.ConversationId, participant);
        }
    }
}

using AutoMapper;
using MediatR;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.MongoEntities.Conversations;

namespace Vogel.Messanger.Application.Conversations.Handlers
{
    public class ParticipantSynchronizationHandler :
        INotificationHandler<EntityCreatedEvent<Participant>>,
        INotificationHandler<EntityUpdatedEvent<Participant>>
    {
        private readonly IMongoRepository<ParticipantMongoEntity> _participantMongoRepository;
        private readonly IMapper _mapper;

        public ParticipantSynchronizationHandler(IMongoRepository<ParticipantMongoEntity> participantMongoRepository, IMapper mapper)
        {
            _participantMongoRepository = participantMongoRepository;
            _mapper = mapper;
        }

        public async Task Handle(EntityCreatedEvent<Participant> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<Participant, ParticipantMongoEntity>(notification.Entity);

            await _participantMongoRepository.ReplaceOrInsertAsync(mongoEntity);
        }

        public async Task Handle(EntityUpdatedEvent<Participant> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<Participant, ParticipantMongoEntity>(notification.Entity);

            await _participantMongoRepository.ReplaceOrInsertAsync(mongoEntity);
        }
    }
}

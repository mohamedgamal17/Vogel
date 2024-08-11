using AutoMapper;
using MediatR;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.Domain.Medias;
using Vogel.MongoDb.Entities.Medias;

namespace Vogel.Application.Medias.EventHandlers
{
    public class MongoSynchronizationHandler :
        INotificationHandler<EntityCreatedEvent<Media>>,
        INotificationHandler<EntityUpdatedEvent<Media>>,
        INotificationHandler<EntityDeletedEvent<Media>>
    {
        private readonly IMapper _mapper;

        private readonly MediaMongoRepository _mediaMongoRepository;

        public MongoSynchronizationHandler(IMapper mapper, MediaMongoRepository mediaMongoRepository)
        {
            _mapper = mapper;
            _mediaMongoRepository = mediaMongoRepository;
        }

        public async Task Handle(EntityCreatedEvent<Media> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<Media, MediaMongoEntity>(notification.Entity);

            await _mediaMongoRepository.InsertAsync(mongoEntity);

        }

        public async Task Handle(EntityUpdatedEvent<Media> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<Media, MediaMongoEntity>(notification.Entity);

            await _mediaMongoRepository.UpdateAsync(mongoEntity);

        }

        public async Task Handle(EntityDeletedEvent<Media> notification, CancellationToken cancellationToken)
        {
            await _mediaMongoRepository.DeleteAsync(notification.Entity.Id);
        }
    }
}

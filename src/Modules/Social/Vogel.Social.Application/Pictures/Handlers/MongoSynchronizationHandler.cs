using AutoMapper;
using MediatR;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.Social.Domain.Pictures;
using Vogel.Social.MongoEntities.Pictures;

namespace Vogel.Social.Application.Pictures.Handlers
{
    internal class MongoSynchronizationHandler :
        INotificationHandler<EntityCreatedEvent<Picture>>,
        INotificationHandler<EntityUpdatedEvent<Picture>>,
        INotificationHandler<EntityDeletedEvent<Picture>>
    {
        private readonly IMapper _mapper;

        private readonly PictureMongoRepository _pictureMongoRepository;

        public MongoSynchronizationHandler(IMapper mapper, PictureMongoRepository pictureMongoRepository)
        {
            _mapper = mapper;
            _pictureMongoRepository = pictureMongoRepository;
        }

        public async Task Handle(EntityCreatedEvent<Picture> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<Picture, PictureMongoEntity>(notification.Entity);

            await _pictureMongoRepository.InsertAsync(mongoEntity);
        }

        public async Task Handle(EntityUpdatedEvent<Picture> notification, CancellationToken cancellationToken)
        {
            var mongoEntity = _mapper.Map<Picture, PictureMongoEntity>(notification.Entity);

            await _pictureMongoRepository.UpdateAsync(mongoEntity);
        }

        public async Task Handle(EntityDeletedEvent<Picture> notification, CancellationToken cancellationToken)
        {
            await _pictureMongoRepository.DeleteAsync(notification.Entity.Id);
        }
    }
}

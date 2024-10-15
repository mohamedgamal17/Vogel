using MassTransit;
using MediatR;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.Social.Domain.Users;
using Vogel.Social.MongoEntities.Pictures;
using Vogel.Social.Shared.Events;
using Vogel.Social.Shared.Events.Models;
namespace Vogel.Social.Application.Users.Handlers
{
    public class IntegrationEventPublisherHandler :
        INotificationHandler<EntityCreatedEvent<User>>,
        INotificationHandler<EntityUpdatedEvent<User>>
    {
        private readonly IPublishEndpoint _publishEndPoint;
        private readonly IMongoRepository<PictureMongoEntity> _pictureMongoRepository;
        public IntegrationEventPublisherHandler(IPublishEndpoint publishEndPoint, IMongoRepository<PictureMongoEntity> pictureMongoRepository)
        {
            _publishEndPoint = publishEndPoint;
            _pictureMongoRepository = pictureMongoRepository;
        }

        public async Task Handle(EntityCreatedEvent<User> notification, CancellationToken cancellationToken)
        {
            var @event = new UserCreatedIntegrationEvent
            {
                Id = notification.Entity.Id,
                FirstName = notification.Entity.FirstName,
                LastName = notification.Entity.LastName,
                AvatarId = notification.Entity.AvatarId,
                BirthDate = notification.Entity.BirthDate,
                Gender = notification.Entity.Gender
            };

            if (notification.Entity.AvatarId != null)
            {
                var avatar = await _pictureMongoRepository.FindByIdAsync(notification.Entity.AvatarId);

                @event.Avatar = new PictureModel
                {
                    Id = avatar!.Id,
                    File = avatar.File,
                    UserId = avatar.UserId
                };
            }

            await _publishEndPoint.Publish(@event);
        }

        public async Task Handle(EntityUpdatedEvent<User> notification, CancellationToken cancellationToken)
        {
            var @event = new UserUpdatedIntegrationEvent
            {
                Id = notification.Entity.Id,
                FirstName = notification.Entity.FirstName,
                LastName = notification.Entity.LastName,
                AvatarId = notification.Entity.AvatarId,
                BirthDate = notification.Entity.BirthDate,
                Gender = notification.Entity.Gender
            };

            if (notification.Entity.AvatarId != null)
            {
                var avatar = await _pictureMongoRepository.FindByIdAsync(notification.Entity.AvatarId);

                @event.Avatar = new PictureModel
                {
                    Id = avatar!.Id,
                    File = avatar.File,
                    UserId = avatar.UserId
                };
            }

            await _publishEndPoint.Publish(@event);
        }
    }
}

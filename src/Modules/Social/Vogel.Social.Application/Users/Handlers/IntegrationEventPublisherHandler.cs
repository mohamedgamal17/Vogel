using MassTransit;
using MediatR;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.Social.Domain.Users;
using Vogel.Social.Shared.Events;

namespace Vogel.Social.Application.Users.Handlers
{
    public class IntegrationEventPublisherHandler :
        INotificationHandler<EntityCreatedEvent<User>>,
        INotificationHandler<EntityUpdatedEvent<User>>
    {
        private readonly IPublishEndpoint _publishEndPoint;

        public IntegrationEventPublisherHandler(IPublishEndpoint publishEndPoint)
        {
            _publishEndPoint = publishEndPoint;
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

            await _publishEndPoint.Publish(@event);
        }
    }
}

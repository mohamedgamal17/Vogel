using MassTransit;
using MediatR;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.MediaEngine.Shared.Services;
using Vogel.Social.Domain.Users;
using Vogel.Social.Shared.Events;
namespace Vogel.Social.Application.Users.Handlers
{
    public class IntegrationEventPublisherHandler :
        INotificationHandler<EntityCreatedEvent<User>>,
        INotificationHandler<EntityUpdatedEvent<User>>
    {
        private readonly IPublishEndpoint _publishEndPoint;
        private readonly IMediaService _mediaService;
        public IntegrationEventPublisherHandler(IPublishEndpoint publishEndPoint, IMediaService mediaService)
        {
            _publishEndPoint = publishEndPoint;
            _mediaService = mediaService;
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
                var avatarResult = await _mediaService.GetPublicMediaById(notification.Entity.AvatarId);
                if (avatarResult.IsSuccess)
                {
                    @event.Avatar = avatarResult.Value;
                }
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
                var avatarResult = await _mediaService.GetPublicMediaById(notification.Entity.AvatarId);
                if (avatarResult.IsSuccess)
                {
                    @event.Avatar = avatarResult.Value;
                }
            }

            await _publishEndPoint.Publish(@event);
        }
    }
}

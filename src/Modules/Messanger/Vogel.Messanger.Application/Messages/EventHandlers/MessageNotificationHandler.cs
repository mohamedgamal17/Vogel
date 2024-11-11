using MassTransit;
using MediatR;
using Vogel.BuildingBlocks.Domain.Events;
using Vogel.Messanger.Application.Messages.Events;
using Vogel.Messanger.Domain.Messages;

namespace Vogel.Messanger.Application.Messages.EventHandlers
{
    public class MessageNotificationHandler : INotificationHandler<EntityCreatedEvent<Message>>
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MessageNotificationHandler(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(EntityCreatedEvent<Message> notification, CancellationToken cancellationToken)
        {

            var @event = new MessageSentEvent
            {
                Message = notification.Entity
            };

            await _publishEndpoint.Publish(@event);
        }
    }
}

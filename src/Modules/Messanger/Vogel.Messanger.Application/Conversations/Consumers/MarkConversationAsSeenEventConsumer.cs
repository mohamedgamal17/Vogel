using MassTransit;
using MediatR;
using Vogel.Messanger.Application.Conversations.Commands.MarkConversationAsSeen;
using Vogel.Messanger.Application.Conversations.Events;
namespace Vogel.Messanger.Application.Conversations.Consumers
{
    public class MarkConversationAsSeenEventConsumer : IConsumer<MarkConversationAsSeenEvent>
    {
        private readonly IMediator _mediator;

        public MarkConversationAsSeenEventConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<MarkConversationAsSeenEvent> context)
        {
            var command = new MarkConversationAsSeenCommand
            {
                ConversationId = context.Message.ConversationId,
                UserId = context.Message.UserId
            };

            var result = await _mediator.Send(command);
        }
    }
}

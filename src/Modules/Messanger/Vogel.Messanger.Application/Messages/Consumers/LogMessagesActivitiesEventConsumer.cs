using MassTransit;
using Vogel.Messanger.Application.Messages.Events;
using Vogel.Messanger.MongoEntities.Messages;

namespace Vogel.Messanger.Application.Messages.Consumers
{
    public class LogMessagesActivitiesEventConsumer : IConsumer<LogMessagesActivitiesEvent>
    {
        private readonly MessageMongoRepository _messageMongoRepository;

        public LogMessagesActivitiesEventConsumer(MessageMongoRepository messageMongoRepository)
        {
            _messageMongoRepository = messageMongoRepository;
        }

        public async Task Consume(ConsumeContext<LogMessagesActivitiesEvent> context)
        {
            await _messageMongoRepository.LogConversationMessages(context.Message.ConversationId, context.Message.SeenById,
                 context.Message.SeentAt);
        }
    }
}

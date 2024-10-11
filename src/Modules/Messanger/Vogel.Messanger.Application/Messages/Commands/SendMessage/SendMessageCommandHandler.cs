using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.MongoDb;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Messanger.Application.Messages.Dtos;
using Vogel.Messanger.Application.Messages.Factories;
using Vogel.Messanger.Domain;
using Vogel.Messanger.Domain.Messages;
using Vogel.Messanger.MongoEntities.Messages;
namespace Vogel.Messanger.Application.Messages.Commands.SendMessage
{
    public class SendMessageCommandHandler : IApplicationRequestHandler<SendMessageCommand, MessageDto>
    {
        private readonly IMessangerRepository<Message> _messageRepository;
        private readonly IMongoRepository<MessageMongoEntity> _messageMongoRepository;
        private readonly IMessageResponseFactory _messageResponseFactory;
        private readonly ISecurityContext _securityContext;

        public SendMessageCommandHandler(IMessangerRepository<Message> messageRepository, IMongoRepository<MessageMongoEntity> messageMongoRepository, IMessageResponseFactory messageResponseFactory, ISecurityContext securityContext)
        {
            _messageRepository = messageRepository;
            _messageMongoRepository = messageMongoRepository;
            _messageResponseFactory = messageResponseFactory;
            _securityContext = securityContext;
        }

        public async Task<Result<MessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var message = new Message
            {
                SenderId = _securityContext.User!.Id,
                ReciverId = request.ReciverId,
                Content = request.Content,
            };

            await _messageRepository.InsertAsync(message);

            var mongoEntity = await  _messageMongoRepository.FindByIdAsync(message.Id);

            return await _messageResponseFactory.PrepareMessageDto(mongoEntity!);
        }
    }
}

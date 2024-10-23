using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Messanger.Application.Conversations.Policies;
using Vogel.Messanger.Application.Messages.Dtos;
using Vogel.Messanger.Application.Messages.Factories;
using Vogel.Messanger.Domain;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.Domain.Messages;
using Vogel.Messanger.MongoEntities.Messages;
namespace Vogel.Messanger.Application.Messages.Commands.SendMessage
{
    public class SendMessageCommandHandler : IApplicationRequestHandler<SendMessageCommand, MessageDto>
    {
        private readonly IMessangerRepository<Message> _messageRepository;
        private readonly MessageMongoRepository _messageMongoRepository;
        private readonly IMessangerRepository<Conversation> _conversationRepository;
        private readonly IMessageResponseFactory _messageResponseFactory;
        private readonly ISecurityContext _securityContext;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public SendMessageCommandHandler(IMessangerRepository<Message> messageRepository, MessageMongoRepository messageMongoRepository, IMessangerRepository<Conversation> conversationRepository, IMessageResponseFactory messageResponseFactory, ISecurityContext securityContext, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _messageRepository = messageRepository;
            _messageMongoRepository = messageMongoRepository;
            _conversationRepository = conversationRepository;
            _messageResponseFactory = messageResponseFactory;
            _securityContext = securityContext;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<MessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            var conversation = await _conversationRepository.FindByIdAsync(request.ConversationId);

            if(conversation == null)
            {
                return new Result<MessageDto>(new EntityNotFoundException(typeof(Conversation), request.ConversationId));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(conversation, ConversationOperationalRequirments.IsParticipant);

            if (authorizationResult.IsFailure)
            {
                return new Result<MessageDto>(authorizationResult.Exception!);
            }

            var message = new Message
            {
                SenderId = _securityContext.User!.Id,
                ConversationId = conversation.Id,
                Content = request.Content,
            };

            await _messageRepository.InsertAsync(message);

            var mongoEntity = await _messageMongoRepository.GetMessageViewbyId(conversation.Id, message.Id);

            return await _messageResponseFactory.PrepareMessageDto(mongoEntity!);
        }
    }
}

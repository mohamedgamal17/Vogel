using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Messanger.Application.Conversations.Policies;
using Vogel.Messanger.Application.Messages.Dtos;
using Vogel.Messanger.Application.Messages.Factories;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.Domain.Messages;
using Vogel.Messanger.MongoEntities.Conversations;
using Vogel.Messanger.MongoEntities.Messages;

namespace Vogel.Messanger.Application.Messages.Queries.GetUserConversationMessage
{
    public class GetUserConversationMessageQueryHandler : IApplicationRequestHandler<GetUserConversationMessageQuery, MessageDto>
    {
        private readonly ConversationMongoRepository _conversationMongoRepository;
        private readonly MessageMongoRepository _messageMongoRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly MessageResponseFactory _messageResponseFactory;

        public GetUserConversationMessageQueryHandler(ConversationMongoRepository conversationMongoRepository, MessageMongoRepository messageMongoRepository, IApplicationAuthorizationService applicationAuthorizationService, MessageResponseFactory messageResponseFactory)
        {
            _conversationMongoRepository = conversationMongoRepository;
            _messageMongoRepository = messageMongoRepository;
            _applicationAuthorizationService = applicationAuthorizationService;
            _messageResponseFactory = messageResponseFactory;
        }

        public async Task<Result<MessageDto>> Handle(GetUserConversationMessageQuery request, CancellationToken cancellationToken)
        {
            var conversation = await _conversationMongoRepository.FindByIdAsync(request.ConversationId);

            if (conversation == null)
            {
                return new Result<MessageDto>(new EntityNotFoundException(typeof(Conversation), request.ConversationId));
            }


            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(new IsParticipantInConversationRequirment { ConversationId = request.ConversationId });


            if (authorizationResult.IsFailure)
            {
                return new Result<MessageDto>(authorizationResult.Exception!);
            }

            var result = await _messageMongoRepository.FindViewAsync(request.ConversationId, request.MessageId);

            if(result == null)
            {
                return new Result<MessageDto>(new EntityNotFoundException(typeof(Message), request.MessageId));
            }


            return await _messageResponseFactory.PrepareMessageDto(result);
        }
    }
}

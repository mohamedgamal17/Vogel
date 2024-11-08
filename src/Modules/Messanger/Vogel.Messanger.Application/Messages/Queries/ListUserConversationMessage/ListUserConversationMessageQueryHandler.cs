using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Messanger.Application.Conversations.Policies;
using Vogel.Messanger.Application.Messages.Dtos;
using Vogel.Messanger.Application.Messages.Factories;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.MongoEntities.Conversations;
using Vogel.Messanger.MongoEntities.Messages;

namespace Vogel.Messanger.Application.Messages.Queries.ListUserConversationMessage
{
    public class ListUserConversationMessageQueryHandler : IApplicationRequestHandler<ListUserConversationMessageQuery, Paging<MessageDto>>
    {
        private readonly ConversationMongoRepository _conversationMongoRepository;
        private readonly MessageMongoRepository _messageMongoRepository;
        private readonly IApplicationAuthorizationService _applicationAuthorizationSerivce;
        private readonly MessageResponseFactory _messageResponseFactory;

        public ListUserConversationMessageQueryHandler(ConversationMongoRepository conversationMongoRepository, MessageMongoRepository messageMongoRepository, IApplicationAuthorizationService applicationAuthorizationSerivce, MessageResponseFactory messageResponseFactory)
        {
            _conversationMongoRepository = conversationMongoRepository;
            _messageMongoRepository = messageMongoRepository;
            _applicationAuthorizationSerivce = applicationAuthorizationSerivce;
            _messageResponseFactory = messageResponseFactory;
        }

        public async Task<Result<Paging<MessageDto>>> Handle(ListUserConversationMessageQuery request, CancellationToken cancellationToken)
        {
            var conversation = await _conversationMongoRepository.FindByIdAsync(request.ConversationId);

            if (conversation == null)
            {
                return new Result<Paging<MessageDto>>(new EntityNotFoundException(typeof(Conversation), request.ConversationId));
            }

            var authorizationResult = await _applicationAuthorizationSerivce.AuthorizeAsync(new IsParticipantInConversationRequirment { ConversationId = request.ConversationId });


            if (authorizationResult.IsFailure)
            {
                return new Result<Paging<MessageDto>>(authorizationResult.Exception!);
            }

            var result = await _messageMongoRepository.GetPagedMessagesView(request.ConversationId, request.Cursor, request.Asending, request.Limit);

            var paged = new Paging<MessageDto>
            {
                Data = await _messageResponseFactory.PrepareListMessageDto(result.Data),
                Info = result.Info
            };

            throw paged;
        }
    }
}

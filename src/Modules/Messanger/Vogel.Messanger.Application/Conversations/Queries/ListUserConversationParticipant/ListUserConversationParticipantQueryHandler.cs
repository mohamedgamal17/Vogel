using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.Application.Conversations.Factories;
using Vogel.Messanger.Application.Conversations.Policies;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.MongoEntities.Conversations;

namespace Vogel.Messanger.Application.Conversations.Queries.ListUserConversationParticipant
{
    public class ListUserConversationParticipantQueryHandler : IApplicationRequestHandler<ListUserConversationParticipantQuery, Paging<ParticipantDto>>
    {
        private readonly ConversationMongoRepository _conversationMongoRepository;
        private readonly ParticipantMongoRepository _participantMongoRepository;
        private readonly IParticipantResponseFactory _participantResponseFactory;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public ListUserConversationParticipantQueryHandler(ConversationMongoRepository conversationMongoRepository, ParticipantMongoRepository participantMongoRepository, IParticipantResponseFactory participantResponseFactory, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _conversationMongoRepository = conversationMongoRepository;
            _participantMongoRepository = participantMongoRepository;
            _participantResponseFactory = participantResponseFactory;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<Paging<ParticipantDto>>> Handle(ListUserConversationParticipantQuery request, CancellationToken cancellationToken)
        {
            var conversation = await _conversationMongoRepository.FindByIdAsync(request.ConversationId);

            if(conversation == null)
            {
                return new Result<Paging<ParticipantDto>>(new EntityNotFoundException(typeof(Conversation), request.ConversationId));
            }


            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(new IsParticipantInConversationRequirment { ConversationId = conversation.Id });

            if (authorizationResult.IsFailure)
            {
                return new Result<Paging<ParticipantDto>>(authorizationResult.Exception!);
            }

            var participants = await _participantMongoRepository.GetConversationParticipantsPaged(request.ConversationId, request.Cursor, request.Limit);

            var paged = new Paging<ParticipantDto>
            {
                Data = await _participantResponseFactory.PrepareListParticipantDto(participants.Data),
                Info = participants.Info
            };

            return paged;
        }
    }
}

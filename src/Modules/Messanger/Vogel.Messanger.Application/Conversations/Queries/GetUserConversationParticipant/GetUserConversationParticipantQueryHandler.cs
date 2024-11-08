using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.Application.Conversations.Factories;
using Vogel.Messanger.Application.Conversations.Policies;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.MongoEntities.Conversations;

namespace Vogel.Messanger.Application.Conversations.Queries.GetUserConversationParticipant
{
    public class GetUserConversationParticipantQueryHandler : IApplicationRequestHandler<GetUserConversationParticipantQuery, ParticipantDto>
    {
        private readonly ConversationMongoRepository _conversationMongoRepository;
        private readonly ParticipantMongoRepository _participantMongoRepository;
        private readonly ParticipantResponseFactory _participantResponseFactory;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;

        public GetUserConversationParticipantQueryHandler(ConversationMongoRepository conversationMongoRepository, ParticipantMongoRepository participantMongoRepository, ParticipantResponseFactory participantResponseFactory, IApplicationAuthorizationService applicationAuthorizationService)
        {
            _conversationMongoRepository = conversationMongoRepository;
            _participantMongoRepository = participantMongoRepository;
            _participantResponseFactory = participantResponseFactory;
            _applicationAuthorizationService = applicationAuthorizationService;
        }

        public async Task<Result<ParticipantDto>> Handle(GetUserConversationParticipantQuery request, CancellationToken cancellationToken)
        {
            var conversation = await _conversationMongoRepository.FindByIdAsync(request.ConversationId);

            if(conversation == null)
            {
                return new Result<ParticipantDto>(new EntityNotFoundException(typeof(Conversation), request.ConversationId));
            }

            var authorizationResult = await _applicationAuthorizationService.AuthorizeAsync(new IsParticipantInConversationRequirment { ConversationId = conversation.Id });

            if (authorizationResult.IsFailure)
            {
                return new Result<ParticipantDto>(authorizationResult.Exception!);
            }

            var participant = await _participantMongoRepository.GetConversationParticipant(request.ConversationId, request.ParticipantId);

            if(participant == null)
            {
                return new Result<ParticipantDto>(new EntityNotFoundException(typeof(Participant), request.ParticipantId));
            }

            return await _participantResponseFactory.PrepareParticipantDto(participant);
        }
    }
}

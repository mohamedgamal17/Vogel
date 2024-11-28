using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.Application.Conversations.Factories;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.MongoEntities.Conversations;
namespace Vogel.Messanger.Application.Conversations.Queries.GetUserConversationParticipant
{
    public class GetUserConversationParticipantQueryHandler : IApplicationRequestHandler<GetUserConversationParticipantQuery, ParticipantDto>
    {
        private readonly ConversationMongoRepository _conversationMongoRepository;
        private readonly ISecurityContext _securityContext;
        private readonly ParticipantResponseFactory _participantResponseFactory;

        public GetUserConversationParticipantQueryHandler(ConversationMongoRepository conversationMongoRepository, ISecurityContext securityContext, ParticipantResponseFactory participantResponseFactory)
        {
            _conversationMongoRepository = conversationMongoRepository;
            _securityContext = securityContext;
            _participantResponseFactory = participantResponseFactory;
        }

        public async Task<Result<ParticipantDto>> Handle(GetUserConversationParticipantQuery request, CancellationToken cancellationToken)
        {
            string userId = _securityContext.User!.Id;

            var conversation = await _conversationMongoRepository.FindViewAsync(userId, request.ConversationId);

            if(conversation == null)
            {
                return new Result<ParticipantDto>(new EntityNotFoundException(typeof(Conversation), request.ConversationId));
            }

            var participant = await _conversationMongoRepository.FindParticipantAsync(request.ConversationId, request.ParticipantId);

            if(participant == null)
            {
                return new Result<ParticipantDto>(new EntityNotFoundException(typeof(Participant), request.ParticipantId));
            }

            return await _participantResponseFactory.PrepareParticipantDto(participant);
        }
    }
}

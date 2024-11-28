using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.Application.Conversations.Factories;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.MongoEntities.Conversations;

namespace Vogel.Messanger.Application.Conversations.Queries.ListUserConversationParticipant
{
    public class ListUserConversationParticipantQueryHandler : IApplicationRequestHandler<ListUserConversationParticipantQuery, Paging<ParticipantDto>>
    {
        private readonly ConversationMongoRepository _conversationMongoRepository;
        private readonly ISecurityContext _securityContext;
        private readonly IParticipantResponseFactory _participantResponseFactory;

        public ListUserConversationParticipantQueryHandler(ConversationMongoRepository conversationMongoRepository,  ISecurityContext securityContext, IParticipantResponseFactory participantResponseFactory)
        {
            _conversationMongoRepository = conversationMongoRepository;
            _securityContext = securityContext;
            _participantResponseFactory = participantResponseFactory;
        }

        public async Task<Result<Paging<ParticipantDto>>> Handle(ListUserConversationParticipantQuery request, CancellationToken cancellationToken)
        {
            string userId = _securityContext.User!.Id;

            var conversation = await _conversationMongoRepository.FindViewAsync(userId, request.ConversationId);

            if (conversation == null)
            {
                return new Result<Paging<ParticipantDto>>(new EntityNotFoundException(typeof(Conversation), request.ConversationId));
            }

            var participants = await _conversationMongoRepository.QueryParticipantAsync(request.ConversationId, request.Cursor, request.Limit, request.Asending);


            var paged = new Paging<ParticipantDto>
            {
                Data = await _participantResponseFactory.PrepareListParticipantDto(participants.Data),
                Info = participants.Info
            };

            return paged;
        }
    }
}

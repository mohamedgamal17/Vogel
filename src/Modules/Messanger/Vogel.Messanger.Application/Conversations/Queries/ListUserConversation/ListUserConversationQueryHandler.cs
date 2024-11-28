using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.Application.Conversations.Factories;
using Vogel.Messanger.MongoEntities.Conversations;

namespace Vogel.Messanger.Application.Conversations.Queries.ListUserConversation
{
    public class ListUserConversationQueryHandler : IApplicationRequestHandler<ListUserConversationQuery, Paging<ConversationDto>>
    {
        private readonly ConversationMongoRepository _conversationMongoRepository;
        private readonly ISecurityContext _securityContext;
        private readonly IConversationResponseFactory _conversationResponseFactory;

        public ListUserConversationQueryHandler(ConversationMongoRepository conversationMongoRepository, ISecurityContext securityContext, IConversationResponseFactory conversationResponseFactory)
        {
            _conversationMongoRepository = conversationMongoRepository;
            _securityContext = securityContext;
            _conversationResponseFactory = conversationResponseFactory;
        }

        public async Task<Result<Paging<ConversationDto>>> Handle(ListUserConversationQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _securityContext.User!.Id;

            var result = await _conversationMongoRepository.QueryViewAsync(currentUserId, request.Cursor, request.Limit , request.Asending);

            var paged = new Paging<ConversationDto>
            {
                Data = await _conversationResponseFactory.PrepareListConversationDto(result.Data),
                Info = result.Info
            };

            return paged;
        }
    }
}

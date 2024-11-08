using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Domain.Exceptions;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.BuildingBlocks.Shared.Results;
using Vogel.Messanger.Application.Conversations.Dtos;
using Vogel.Messanger.Application.Conversations.Factories;
using Vogel.Messanger.Application.Conversations.Policies;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.MongoEntities.Conversations;

namespace Vogel.Messanger.Application.Conversations.Queries.GetUserConversationById
{
    public class GetConversationByIdQueryHandler : IApplicationRequestHandler<GetConversationByIdQuery, ConversationDto>
    {
        private readonly ConversationMongoRepository _conversationMongoRepository;
        private readonly ISecurityContext _securityContext;
        private readonly IApplicationAuthorizationService _applicationAuthorizationService;
        private readonly IConversationResponseFactory _conversationResponseFactory;

        public GetConversationByIdQueryHandler(ConversationMongoRepository conversationMongoRepository, ISecurityContext securityContext, IApplicationAuthorizationService applicationAuthorizationService, IConversationResponseFactory conversationResponseFactory)
        {
            _conversationMongoRepository = conversationMongoRepository;
            _securityContext = securityContext;
            _applicationAuthorizationService = applicationAuthorizationService;
            _conversationResponseFactory = conversationResponseFactory;
        }

        public async Task<Result<ConversationDto>> Handle(GetConversationByIdQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _securityContext.User!.Id;

            var conversation = await _conversationMongoRepository.GetConversationViewById(currentUserId);

            if(conversation == null)
            {
                return new Result<ConversationDto>(new EntityNotFoundException(typeof(Conversation), request.ConversationId));
            }

            var authorizationResult = await _applicationAuthorizationService
                .AuthorizeAsync(new IsParticipantInConversationRequirment() { ConversationId = request.ConversationId});

            if (authorizationResult.IsFailure)
            {
                return new Result<ConversationDto>(authorizationResult.Exception!);
            }

            return await _conversationResponseFactory.PrepareConversationDto(conversation);
        }
    }
}

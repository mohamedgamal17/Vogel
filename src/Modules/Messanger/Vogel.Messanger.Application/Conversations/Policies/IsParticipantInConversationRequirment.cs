using Microsoft.AspNetCore.Authorization;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.Domain;
using Vogel.BuildingBlocks.Infrastructure.Security;
namespace Vogel.Messanger.Application.Conversations.Policies
{
    public class IsParticipantInConversationRequirment : IAuthorizationRequirement
    {
        public string ConversationId { get; set; }
        public string UserId { get; set; }
    }
    public class IsParticipantInConversationRequirmentHandler : AuthorizationHandler<IsParticipantInConversationRequirment>
    {
        private readonly ISecurityContext _securityContext;
        private readonly IMessangerRepository<Participant> _participantRepository;

        public IsParticipantInConversationRequirmentHandler(ISecurityContext securityContext, IMessangerRepository<Participant> participantRepository)
        {
            _securityContext = securityContext;
            _participantRepository = participantRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsParticipantInConversationRequirment requirement)
        {
            var currentUserId =  requirement.UserId;

            if(currentUserId == null)
            {
                currentUserId = _securityContext.User?.Id;
            }

            if(currentUserId == null)
            {
                throw new InvalidOperationException($"Current user id in ({typeof(IsParticipantInConversationRequirmentHandler).AssemblyQualifiedName}) , cannot be null user must be authenticated , or UserId Property in $({typeof(IsParticipantInConversationRequirment).AssemblyQualifiedName}) should be assigned");
            }

            var participant = await _participantRepository.SingleOrDefaultAsync(x => x.ConversationId == requirement.ConversationId && x.UserId == currentUserId);

            if (participant != null)
            {
                context.Succeed(requirement);
            }

        }
    }
}

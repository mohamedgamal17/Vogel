using Microsoft.AspNetCore.Authorization;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.Domain;
using Vogel.BuildingBlocks.Infrastructure.Security;
namespace Vogel.Messanger.Application.Conversations.Policies
{
    public class IsParticipantInConversationRequirment : IAuthorizationRequirement
    {
        public string ConversationId { get; set; }
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
            var currentUserId = _securityContext.User!.Id;

            var participant = await _participantRepository.SingleOrDefaultAsync(x => x.ConversationId == requirement.ConversationId && x.UserId == currentUserId);

            if (participant != null)
            {
                context.Succeed(requirement);
            }

        }
    }
}

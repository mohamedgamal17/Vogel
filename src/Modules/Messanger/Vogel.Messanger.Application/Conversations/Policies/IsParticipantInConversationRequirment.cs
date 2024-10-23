using Microsoft.AspNetCore.Authorization;
using Vogel.Messanger.Domain.Conversations;
using Vogel.Messanger.Domain;
using Vogel.BuildingBlocks.Infrastructure.Security;
namespace Vogel.Messanger.Application.Conversations.Policies
{
    public class IsParticipantInConversationRequirment : IAuthorizationRequirement
    {

    }
    public class IsParticipantInConversationRequirmentHandler : AuthorizationHandler<IsParticipantInConversationRequirment, Conversation>
    {
        private readonly ISecurityContext _securityContext;
        private readonly IMessangerRepository<Participant> _participantRepository;

        public IsParticipantInConversationRequirmentHandler(ISecurityContext securityContext, IMessangerRepository<Participant> participantRepository)
        {
            _securityContext = securityContext;
            _participantRepository = participantRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsParticipantInConversationRequirment requirement, Conversation resource)
        {
            var currentUserId = _securityContext.User!.Id;

            var participant = await _participantRepository.SingleOrDefaultAsync(x => x.ConversationId == resource.Id && x.UserId == currentUserId);

            if (participant != null)
            {
                context.Succeed(requirement);
            }

        }
    }
}

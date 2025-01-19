using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Messanger.Application.Conversations.Dtos;

namespace Vogel.Messanger.Application.Conversations.Queries.GetUserConversationParticipant
{
    [Authorize]
    public class GetUserConversationParticipantQuery : IQuery<ParticipantDto>
    {
        public string ConversationId { get; set; }
        public string ParticipantId { get; set; }
    }
}

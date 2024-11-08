using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Messanger.Application.Conversations.Dtos;

namespace Vogel.Messanger.Application.Conversations.Queries.ListUserConversationParticipant
{
    [Authorize]
    public class ListUserConversationParticipantQuery : PagingParams ,IQuery<Paging<ParticipantDto>>
    {
        public string ConversationId { get; set; }
    }
}

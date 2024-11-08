using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Messanger.Application.Conversations.Dtos;

namespace Vogel.Messanger.Application.Conversations.Queries.ListUserConversation
{
    [Authorize]
    public class ListUserConversationQuery : PagingParams, IQuery<Paging<ConversationDto>> 
    {
    }
}

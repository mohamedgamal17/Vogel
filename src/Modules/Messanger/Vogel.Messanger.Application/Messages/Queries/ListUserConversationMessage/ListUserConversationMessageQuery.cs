using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Messanger.Application.Messages.Dtos;

namespace Vogel.Messanger.Application.Messages.Queries.ListUserConversationMessage
{
    public class ListUserConversationMessageQuery : PagingParams, IQuery<Paging<MessageDto>>
    {
        public string ConversationId { get; set; }
    }
}

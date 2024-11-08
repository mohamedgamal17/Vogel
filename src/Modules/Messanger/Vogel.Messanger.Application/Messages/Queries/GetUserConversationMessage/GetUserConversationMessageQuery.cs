using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Messanger.Application.Messages.Dtos;
namespace Vogel.Messanger.Application.Messages.Queries.GetUserConversationMessage
{
    public class GetUserConversationMessageQuery : IQuery<MessageDto>
    {
        public string ConversationId { get; set; }
        public string MessageId { get; set; }
    }
}

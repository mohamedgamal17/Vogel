using Vogel.BuildingBlocks.Shared.Dtos;
using Vogel.Messanger.Application.Conversations.Dtos;

namespace Vogel.Messanger.Application.Messages.Dtos
{
    public class MessageDto : EntityDto<string>
    {
        public string Content { get; set; }
        public string ConversationId { get; set; }
        public string SenderId { get; set; }
        public ConversationUserDto Sender { get; set; }
        public bool IsSeen { get; set; }
    }
}

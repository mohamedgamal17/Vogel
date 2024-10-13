using Vogel.BuildingBlocks.Shared.Dtos;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Messanger.Application.Conversations.Dtos
{
    public class ParticipantDto : EntityDto<string>
    {
        public string ConversationId { get; set; }
        public string UserId { get; set; }
        public UserDto User { get; set; }
    }
}

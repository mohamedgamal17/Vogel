using Vogel.BuildingBlocks.Shared.Dtos;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Messanger.Application.Messages.Dtos;

namespace Vogel.Messanger.Application.Conversations.Dtos
{
    public class ConversationDto : EntityDto<string>
    {
        public string? Name { get; set; }
        public Paging<ParticipantDto> Participants { get; set; }

        public Paging<MessageDto> Messages { get; set; }
    }
}

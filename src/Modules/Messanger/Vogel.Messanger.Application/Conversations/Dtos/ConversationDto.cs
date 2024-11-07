using Vogel.BuildingBlocks.Shared.Dtos;
using Vogel.BuildingBlocks.Shared.Models;

namespace Vogel.Messanger.Application.Conversations.Dtos
{
    public class ConversationDto : EntityDto<string>
    {
        public string? Name { get; set; }
        public Paging<ParticipantDto> Participants { get; set; }
    }
}

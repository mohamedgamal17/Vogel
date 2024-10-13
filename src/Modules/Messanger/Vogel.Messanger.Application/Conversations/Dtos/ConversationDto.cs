using Vogel.BuildingBlocks.Shared.Dtos;

namespace Vogel.Messanger.Application.Conversations.Dtos
{
    public class ConversationDto : EntityDto<string>
    {
        public string? Name { get; set; }
        public List<ParticipantDto> Participants { get; set; }
    }
}

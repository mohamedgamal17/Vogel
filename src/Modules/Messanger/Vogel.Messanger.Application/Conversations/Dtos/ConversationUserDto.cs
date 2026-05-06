using Vogel.BuildingBlocks.Shared.Dtos;
using Vogel.Social.Shared.Common;

namespace Vogel.Messanger.Application.Conversations.Dtos
{
    public class ConversationUserDto : EntityDto<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public string BirthDate { get; set; }
        public string? Avatar { get; set; }
    }
}

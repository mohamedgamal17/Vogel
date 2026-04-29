using Vogel.BuildingBlocks.Shared.Dtos;
using Vogel.MediaEngine.Shared.Dtos;
using Vogel.Social.Shared.Common;

namespace Vogel.Social.Shared.Dtos
{
    public class UserDto : EntityDto<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public string BirthDate { get; set; }
        public string? AvatarId { get; set; }
        public PublicMediaFileDto? Avatar { get; set; }
    }
}

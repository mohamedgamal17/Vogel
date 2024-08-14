using Vogel.BuildingBlocks.Application.Dtos;
namespace Vogel.Social.Shared.Pictures.Dtos
{
    public class UserDto : EntityDto<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public string BirthDate { get; set; }
        public string? AvatarId { get; set; }
        public PictureDto? Avatar { get; set; }
    }
}

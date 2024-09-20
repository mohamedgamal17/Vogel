using Vogel.BuildingBlocks.Shared.Dtos;
namespace Vogel.Social.Shared.Dtos
{
    public class FriendDto : EntityDto<string>
    {
        public string SourceId { get; set; }
        public string TargetId { get; set; }
        public UserDto Source { get; set; }
        public UserDto Target { get; set; }
    }
}

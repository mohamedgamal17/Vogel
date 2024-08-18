using Vogel.Application.Users.Dtos;
using Vogel.BuildingBlocks.Shared.Dtos;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.Application.Friendship.Dtos
{
    public class FriendDto : EntityDto<string>
    {
        public string SourceId { get; set; }
        public string TargetId { get; set; }
        public UserDto Source { get; set; }
        public UserDto Target { get; set; }
    }
}

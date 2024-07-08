using Vogel.Application.Users.Dtos;
using Vogel.BuildingBlocks.Application.Dtos;
using Vogel.MongoDb.Entities.Users;

namespace Vogel.Application.Friendship.Dtos
{
    public class FriendDto : EntityDto<string>
    {
        public string SourceId { get; set; }
        public string TargetId { get; set; }
        public PublicUserDto Source { get; set; }
        public PublicUserDto Target { get; set; }
    }
}

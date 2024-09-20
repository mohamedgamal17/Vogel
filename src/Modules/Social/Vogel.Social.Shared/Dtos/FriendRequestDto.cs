using Vogel.BuildingBlocks.Shared.Dtos;
using Vogel.Social.Shared.Common;

namespace Vogel.Social.Shared.Dtos
{
    public class FriendRequestDto : EntityDto<string>
    {
        public string SenderId { get; set; }
        public string ReciverId { get; set; }
        public UserDto Sender { get; set; }
        public UserDto Reciver { get; set; }
        public FriendRequestState State { get; set; }
    }
}

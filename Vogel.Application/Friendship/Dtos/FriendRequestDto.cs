using Vogel.Application.Users.Dtos;
using Vogel.BuildingBlocks.Application.Dtos;
using Vogel.MongoDb.Entities.Friendship;
namespace Vogel.Application.Friendship.Dtos
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

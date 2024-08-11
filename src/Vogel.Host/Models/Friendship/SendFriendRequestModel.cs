using Vogel.Application.Friendship.Commands;

namespace Vogel.Host.Models.Friendship
{
    public class SendFriendRequestModel
    {
        public string ReciverId { get; set; }

        public SendFriendRequestCommand ToSendFriendRequestCommand()
        {
            return new SendFriendRequestCommand { ReciverId = ReciverId };
        }
    }
}

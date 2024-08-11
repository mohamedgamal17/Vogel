using Vogel.BuildingBlocks.Domain.Auditing;

namespace Vogel.Social.Domain.Friendship
{
    public class FriendRequest : AuditedAggregateRoot<string>
    {
        public string SenderId { get; set; }
        public string ReciverId { get; set; }
        public FriendRequestState State { get; private set; }

        public FriendRequest()
        {
            Id = Guid.NewGuid().ToString();
            State = FriendRequestState.Pending;
        }


        public void Accept()
        {
            if (State == FriendRequestState.Pending)
            {
                State = FriendRequestState.Accepted;
            }

        }

        public void Reject()
        {
            if (State == FriendRequestState.Pending)
            {
                State = FriendRequestState.Rejected;

            }
        }

        public void Cancel()
        {
            if (State == FriendRequestState.Pending)
            {
                State = FriendRequestState.Cancelled;
            }
        }
    }
}

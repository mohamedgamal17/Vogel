namespace Vogel.Social.Application.Friendship.Policies
{
    public static class FriendshipOperationalRequirments
    {
        public static FriendRequestSenderActionRequirment FriendRequestSenderAction = new FriendRequestSenderActionRequirment();

        public static FriendRequestReciverActionAuthorizationRequirment FriendRequestReciverAction = new FriendRequestReciverActionAuthorizationRequirment();

        public static IsFriendOwnerRequirment IsFriendOwner = new IsFriendOwnerRequirment();

        public static IsFriendRequestOwnerRequirment IsFriendRequestOwner = new IsFriendRequestOwnerRequirment();
    }
}

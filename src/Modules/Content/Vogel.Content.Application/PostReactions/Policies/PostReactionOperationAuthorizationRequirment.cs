namespace Vogel.Content.Application.PostReactions.Policies
{
    public static class PostReactionOperationAuthorizationRequirment
    {
        public static IsPostReactionOwnerAuthorizationRequirment IsOwner = new IsPostReactionOwnerAuthorizationRequirment();
    }
}

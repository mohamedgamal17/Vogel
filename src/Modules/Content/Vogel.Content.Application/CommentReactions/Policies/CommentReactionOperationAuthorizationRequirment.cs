using Microsoft.AspNetCore.Authorization;

namespace Vogel.Content.Application.CommentReactions.Policies
{
    public class CommentReactionOperationAuthorizationRequirment
    {
        public static IsCommentReactionOwnerAuthorizationRequirment IsOwner = new IsCommentReactionOwnerAuthorizationRequirment();



    }


}

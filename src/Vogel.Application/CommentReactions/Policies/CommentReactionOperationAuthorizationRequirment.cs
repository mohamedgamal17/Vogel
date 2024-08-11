using Microsoft.AspNetCore.Authorization;

namespace Vogel.Application.CommentReactions.Policies
{
    public class CommentReactionOperationAuthorizationRequirment
    {
        public static IsCommentReactionOwnerAuthorizationRequirment IsOwner = new IsCommentReactionOwnerAuthorizationRequirment();



    }


}

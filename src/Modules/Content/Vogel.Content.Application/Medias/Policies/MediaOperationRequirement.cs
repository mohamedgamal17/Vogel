using Microsoft.AspNetCore.Authorization;

namespace Vogel.Content.Application.Medias.Policies
{
    public static class MediaOperationRequirements
    {
        public static IsMediaOwnerRequirment IsOwner = new IsMediaOwnerRequirment();
    }


    public class IsMediaOwnerRequirment : IAuthorizationRequirement { }
}

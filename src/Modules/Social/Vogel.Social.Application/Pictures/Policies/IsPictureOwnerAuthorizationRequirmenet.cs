using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Vogel.Social.Domain.Pictures;
using Vogel.Social.MongoEntities.Pictures;

namespace Vogel.Social.Application.Pictures.Policies
{
    public class IsPictureOwnerAuthorizationRequirmenet : IAuthorizationRequirement
    {

    }

    public class IsPictureOwnerAuthorizationRequirmenetHandler : AuthorizationHandler<IsPictureOwnerAuthorizationRequirmenet, Picture>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsPictureOwnerAuthorizationRequirmenet requirement, Picture resource)
        {
            string userId = context.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (userId == resource.UserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class IsMongoPictureOwnerAuthorizationRequirmenetHandler : AuthorizationHandler<IsPictureOwnerAuthorizationRequirmenet, PictureMongoEntity>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsPictureOwnerAuthorizationRequirmenet requirement, PictureMongoEntity resource)
        {
            string userId = context.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (userId == resource.UserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}

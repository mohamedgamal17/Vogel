﻿using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Vogel.Content.Domain.Medias;
using Vogel.Content.MongoEntities.Medias;
namespace Vogel.Content.Application.Medias.Policies
{
    public class IsMediaOwnerAuthorizationHandler : AuthorizationHandler<IsMediaOwnerRequirment, Media>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsMediaOwnerRequirment requirement, Media resource)
        {
            string userId = context.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (userId == resource.UserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class IsMediaMongoEntityOwnerAuthorizationHandler : AuthorizationHandler<IsMediaOwnerRequirment, MediaMongoEntity>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsMediaOwnerRequirment requirement, MediaMongoEntity resource)
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

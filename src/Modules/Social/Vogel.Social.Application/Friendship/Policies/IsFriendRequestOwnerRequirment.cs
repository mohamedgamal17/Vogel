﻿using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Infrastructure.Security;
using Vogel.Social.Domain.Friendship;
using Vogel.Social.MongoEntities.Friendship;
namespace Vogel.Social.Application.Friendship.Policies
{
    public class IsFriendRequestOwnerRequirment : IAuthorizationRequirement
    {
    }

    public class IsFriendRequestOwnerRequirmentHandler :
        AuthorizationHandler<IsFriendRequestOwnerRequirment, FriendRequest>
    {
        public ISecurityContext _securityContext;

        public IsFriendRequestOwnerRequirmentHandler(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsFriendRequestOwnerRequirment requirement, FriendRequest resource)
        {
            string currentUserId = _securityContext.User!.Id;

            if (resource.SenderId == currentUserId || resource.ReciverId == currentUserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    public class IsFriendRequestMongoViewOwnerRequeirmentHandler : AuthorizationHandler<IsFriendRequestOwnerRequirment, FriendRequestMongoView>
    {
        public ISecurityContext _securityContext;

        public IsFriendRequestMongoViewOwnerRequeirmentHandler(ISecurityContext securityContext)
        {
            _securityContext = securityContext;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsFriendRequestOwnerRequirment requirement, FriendRequestMongoView resource)
        {
            string currentUserId = _securityContext.User!.Id;

            if (resource.SenderId == currentUserId || resource.ReciverId == currentUserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}

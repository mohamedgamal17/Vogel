﻿using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Friendship.Queries.GetFriendRequestById
{
    [Authorize]
    public class GetFriendRequestByIdQuery : IQuery<FriendRequestDto>
    {
        public string FriendRequestId { get; set; }
    }
}

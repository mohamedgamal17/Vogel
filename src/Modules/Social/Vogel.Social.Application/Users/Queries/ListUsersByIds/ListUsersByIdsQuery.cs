﻿using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Social.Application.Users.Queries.ListUsersByIds
{
    [Authorize]
    public class ListUsersByIdsQuery : PagingParams, IQuery<Paging<UserDto>>
    {
        public IEnumerable<string> Ids { get; set; }
    }
}

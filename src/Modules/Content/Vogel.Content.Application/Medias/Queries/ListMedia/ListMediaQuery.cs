﻿using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Content.Application.Medias.Dtos;
namespace Vogel.Content.Application.Medias.Queries.ListMedia
{
    [Authorize]
    public class ListMediaQuery : PagingParams , IQuery<Paging<MediaDto>>
    {

    }
}

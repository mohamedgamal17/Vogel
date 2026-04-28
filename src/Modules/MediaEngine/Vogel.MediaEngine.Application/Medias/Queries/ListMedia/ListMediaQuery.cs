using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.MediaEngine.Shared.Dtos;

namespace Vogel.MediaEngine.Application.Medias.Queries.ListMedia
{
    [Authorize]
    public class ListMediaQuery : PagingParams, IQuery<Paging<MediaDto>>
    {
    }
}

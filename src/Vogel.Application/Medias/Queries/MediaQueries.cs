using Microsoft.AspNetCore.Authorization;
using Vogel.Application.Medias.Dtos;
using Vogel.BuildingBlocks.Application.Requests;

namespace Vogel.Application.Medias.Queries
{
    [Authorize]
    public class ListMediaQuery : IQuery<List<MediaAggregateDto>> { }

    [Authorize]
    public class GetMediaByIdQuery : IQuery<MediaAggregateDto>
    {
        public string Id { get; set; }
    }
}

using Microsoft.AspNetCore.Authorization;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Medias.Dtos;

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

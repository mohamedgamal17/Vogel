using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.MediaEngine.Shared.Dtos;

namespace Vogel.MediaEngine.Application.Medias.Queries.GetMediaById
{
    [Authorize]
    public class GetMediaByIdQuery : IQuery<MediaDto>
    {
        public string MediaId { get; set; }
    }
}

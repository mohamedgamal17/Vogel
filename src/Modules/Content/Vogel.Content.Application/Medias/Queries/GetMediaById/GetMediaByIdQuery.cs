using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.Medias.Dtos;
namespace Vogel.Content.Application.Medias.Queries.GetMediaById
{
    [Authorize]
    public class GetMediaByIdQuery : IQuery<MediaDto> 
    {
        public string MediaId { get; set; }
    }
}

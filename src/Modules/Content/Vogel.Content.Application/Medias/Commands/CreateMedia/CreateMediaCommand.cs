using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.Medias.Dtos;
using Vogel.Content.Domain.Medias;
namespace Vogel.Content.Application.Medias.Commands.CreateMedia
{
    [Authorize]
    public class CreateMediaCommand : ICommand<MediaDto>
    {
        public string Name { get; set; }
        public string MimeType { get; set; }
        public MediaType MediaType { get; set; }
        public Stream Content { get; set; }
    }
}

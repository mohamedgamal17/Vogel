using Microsoft.AspNetCore.Authorization;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Medias.Dtos;
using Vogel.Domain.Medias;

namespace Vogel.Application.Medias.Commands
{
    [Authorize]
    public class CreateMediaCommand : ICommand<MediaAggregateDto>
    {
        public string Name { get; set; }
        public string MimeType { get; set; }
        public MediaType MediaType { get; set; }
        public Stream Content { get; set; }
    }

    [Authorize]
    public class RemoveMediaCommand : ICommand
    {
        public string Id { get; set; }
    }
}

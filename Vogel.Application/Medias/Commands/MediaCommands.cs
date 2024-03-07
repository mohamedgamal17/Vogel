using Vogel.Application.Common.Interfaces;
using Vogel.Application.Medias.Dtos;
using Vogel.Domain;

namespace Vogel.Application.Medias.Commands
{
    public class CreateMediaCommand : ICommand<MediaAggregateDto>
    {
        public string Name { get; set; }
        public string MimeType { get; set; }
        public MediaType MediaType { get; set; }
        public Stream Content { get; set; }
    }

    public class RemoveMediaCommand : ICommand
    {
        public string Id { get; set; }
    }
}

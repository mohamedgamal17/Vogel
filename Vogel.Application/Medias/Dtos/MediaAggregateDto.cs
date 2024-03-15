using Vogel.Application.Common.Dtos;
using Vogel.Domain;

namespace Vogel.Application.Medias.Dtos
{
    public class MediaAggregateDto : EntityDto
    {
        public string Reference { get; set; }
        public MediaType MediaType { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
        public string UserId { get; set; }
    }
}

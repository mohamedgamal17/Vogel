using Vogel.Application.Common.Dtos;
using Vogel.Domain;

namespace Vogel.Application.Medias.Dtos
{
    public class MediaDto : EntityDto
    {
        public string File { get; set; }
        public MediaType MediaType { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
        public string UserId { get; set; }
    }
}

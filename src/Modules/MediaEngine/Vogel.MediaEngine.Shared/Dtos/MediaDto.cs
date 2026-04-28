using Vogel.BuildingBlocks.Shared.Dtos;
using Vogel.MediaEngine.Shared.Enums;

namespace Vogel.MediaEngine.Shared.Dtos
{
    public class MediaDto : EntityDto<string>
    {
        public string File { get; set; }
        public MediaType MediaType { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
        public string UserId { get; set; }
    }
}

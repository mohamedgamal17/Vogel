using Vogel.BuildingBlocks.Shared.Dtos;
using Vogel.Content.MongoEntities.Medias;
namespace Vogel.Content.Application.Medias.Dtos
{
    public class MediaDto : EntityDto<string>
    {
        public string Reference { get; set; }
        public MediaType MediaType { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
        public string UserId { get; set; }
    }
}

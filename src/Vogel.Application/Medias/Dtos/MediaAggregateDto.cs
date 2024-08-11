using Vogel.BuildingBlocks.Application.Dtos;
using Vogel.MongoDb.Entities.Medias;
namespace Vogel.Application.Medias.Dtos
{
    public class MediaAggregateDto : EntityDto<string>
    {
        public string Reference { get; set; }
        public MediaType MediaType { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
        public string UserId { get; set; }
    }
}

using Vogel.BuildingBlocks.Shared.Dtos;

namespace Vogel.Social.Shared.Dtos
{
    public class PictureDto : EntityDto<string>
    {
        public string Reference { get; set; }
        public string UserId { get; set; }
    }
}

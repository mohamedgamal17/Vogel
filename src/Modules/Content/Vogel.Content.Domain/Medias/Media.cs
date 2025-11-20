using Vogel.BuildingBlocks.Domain.Auditing;
namespace Vogel.Content.Domain.Medias
{
    public class Media : OwnedAuditedEntity<string>
    {
        public Media()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string File { get; set; }
        public MediaType MediaType { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
    }

    public enum MediaType
    {
        Image = 0,
        Video = 5
    };
}

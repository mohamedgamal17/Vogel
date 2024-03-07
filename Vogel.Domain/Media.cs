using System.ComponentModel.DataAnnotations;

namespace Vogel.Domain
{
    public class Media : Entity
    {
        public string File { get; set; }
        public MediaType MediaType { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
        public string UserId { get; set; }
        protected override string GetEntityPerfix()
        {
            string perfix = "med";

            return perfix;
        }
    }

    public enum MediaType 
    {
        Image = 0,
        Video = 5
    };
}

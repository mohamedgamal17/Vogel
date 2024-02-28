using System.ComponentModel.DataAnnotations;

namespace Vogel.Domain
{
    public class Media : Entity
    {
        public string File { get; set; }
        public string Extension { get; set; }
        public string MediaType { get; set; }
        public string MimeType { get; set; }

        protected override string GetEntityPerfix()
        {
            string perfix = "med";

            return perfix;
        }
    }
}

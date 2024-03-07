namespace Vogel.Domain
{
    public class Post : Entity
    {
        public string  Caption { get; set; }
        public string? MediaId { get; set; }
        public string UserId { get; set; }
        protected override string GetEntityPerfix()
        {
            string perfix = "pos";

            return perfix;
        }
    }



}

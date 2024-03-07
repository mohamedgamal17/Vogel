namespace Vogel.Domain
{
    public class PostAggregate : Entity
    {
        public string Caption { get; set; }
        public string? MediaId { get; set; }
        public string UserId { get; set; }
        public Media? Media { get; set; }
        public User? User { get; set; }

        protected override string GetEntityPerfix()
        {
            return string.Empty;
        }
    }
}

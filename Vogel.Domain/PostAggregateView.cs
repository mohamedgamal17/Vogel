namespace Vogel.Domain
{
    public class PostAggregateView : Entity
    {
        public string Caption { get; set; }
        public string? MediaId { get; set; }
        public string UserId { get; set; }
        public Media? Media { get; set; }
        public PublicUserView? User { get; set; }

        protected override string GetEntityPerfix()
        {
            return string.Empty;
        }
    }
}

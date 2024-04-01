
namespace Vogel.Domain
{
    public class CommentAggregateView : Entity
    {
        public string Content { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
        public PublicUserView User { get; set; }
        protected override string GetEntityPerfix()
        {
            return string.Empty;
        }
    }
}

using Vogel.BuildingBlocks.Domain.Auditing;
namespace Vogel.Content.Domain.Comments
{
    public class Comment : OwnedAuditedEntity<string>
    {
        public Comment()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Content { get; set; }
        public string PostId { get; set; }
        public string? CommentId { get; set; }
    }
}

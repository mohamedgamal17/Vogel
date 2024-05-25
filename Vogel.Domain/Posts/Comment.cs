using Vogel.BuildingBlocks.Domain;
using Vogel.BuildingBlocks.Domain.Auditing;

namespace Vogel.Domain.Posts
{
    public class Comment : AuditedAggregateRoot<string>
    {
        public Comment()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Content { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
    }
}

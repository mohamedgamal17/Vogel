using Vogel.BuildingBlocks.Domain.Auditing;
using Vogel.Content.Domain.Common;
namespace Vogel.Content.Domain.Posts
{
    public class PostReaction : AuditedAggregateRoot<string>
    {
        public PostReaction()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string UserId { get; set; }
        public string PostId { get; set; }
        public ReactionType Type { get; set; }

    }


}

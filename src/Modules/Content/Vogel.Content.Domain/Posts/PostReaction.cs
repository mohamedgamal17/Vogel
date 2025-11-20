using Vogel.BuildingBlocks.Domain.Auditing;
using Vogel.Content.Domain.Common;
namespace Vogel.Content.Domain.Posts
{
    public class PostReaction : OwnedAuditedAggregateRoot<string>
    {
        public PostReaction()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string PostId { get; set; }
        public ReactionType Type { get; set; }

    }


}

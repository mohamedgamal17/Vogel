using Vogel.BuildingBlocks.Domain.Auditing;
using Vogel.Content.Domain.Common;
namespace Vogel.Content.Domain.Comments
{
    public class CommentReaction : AuditedAggregateRoot<string>
    {
        public CommentReaction()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string CommentId { get; set; }
        public string UserId { get; set; }
        public ReactionType Type { get; set; }

    }
}

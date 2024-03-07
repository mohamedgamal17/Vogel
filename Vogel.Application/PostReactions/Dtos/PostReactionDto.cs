using Vogel.Application.Common.Dtos;
using Vogel.Domain;

namespace Vogel.Application.PostReactions.Dtos
{
    public class PostReactionDto : EntityDto
    {
        public string UserId { get; set; }
        public string PostId { get; set; }
        public ReactionType Type { get; set; }
    }
}

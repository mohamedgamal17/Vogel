using Vogel.BuildingBlocks.Shared.Dtos;
using Vogel.Content.MongoEntities.PostReactions;
using Vogel.Social.Shared.Dtos;

namespace Vogel.Content.Application.PostReactions.Dtos
{
    public class PostReactionDto : EntityDto<string>
    {
        public string UserId { get; set; }
        public string PostId { get; set; }
        public ReactionType Type { get; set; }
        public UserDto User { get; set; }
    }
}

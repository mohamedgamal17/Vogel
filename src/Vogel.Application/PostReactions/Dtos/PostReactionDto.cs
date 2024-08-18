using Vogel.Application.Users.Dtos;
using Vogel.BuildingBlocks.Shared.Dtos;
using Vogel.MongoDb.Entities.PostReactions;

namespace Vogel.Application.PostReactions.Dtos
{
    public class PostReactionDto : EntityDto<string>
    {
        public string UserId { get; set; }
        public string PostId { get; set; }
        public ReactionType Type { get; set; }
        public UserDto User { get; set; }
    }
}

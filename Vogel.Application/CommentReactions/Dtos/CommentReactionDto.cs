using Vogel.Application.Users.Dtos;
using Vogel.BuildingBlocks.Application.Dtos;
using Vogel.MongoDb.Entities.PostReactions;

namespace Vogel.Application.CommentReactions.Dtos
{
    public class CommentReactionDto : EntityDto<string>
    {
        public string UserId { get; set; }
        public string CommentId { get; set; }
        public ReactionType Type { get; set; }
        public UserDto User { get; set; }
    }
}

using Vogel.BuildingBlocks.Shared.Dtos;
using Vogel.Content.MongoEntities.PostReactions;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Content.Application.CommentReactions.Dtos
{
    public class CommentReactionDto : EntityDto<string>
    {
        public string UserId { get; set; }
        public string CommentId { get; set; }
        public ReactionType Type { get; set; }
        public UserDto User { get; set; }
    }
}

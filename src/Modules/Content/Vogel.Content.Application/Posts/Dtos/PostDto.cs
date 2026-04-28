using Vogel.BuildingBlocks.Shared.Dtos;
using Vogel.Content.Application.PostReactions.Dtos;
using Vogel.MediaEngine.Shared.Dtos;
using Vogel.Social.Shared.Dtos;
namespace Vogel.Content.Application.Posts.Dtos
{
    public class PostDto : EntityDto<string>
    {
        public string Caption { get; set; }
        public string? UserId { get; set; }
        public string? MediaId { get; set; }
        public UserDto? User { get; set; }
        public PublicMediaFileDto? Media { get; set; }
        public PostReactionSummaryDto? ReactionSummary { get; set; }
    }

}

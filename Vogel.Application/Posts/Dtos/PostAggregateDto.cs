using Vogel.Application.Medias.Dtos;
using Vogel.Application.PostReactions.Dtos;
using Vogel.Application.Users.Dtos;
using Vogel.BuildingBlocks.Application.Dtos;
namespace Vogel.Application.Posts.Dtos
{
    public class PostAggregateDto : EntityDto<string>
    {
        public string Caption { get; set; }
        public string? UserId { get; set; }
        public string? MediaId { get; set; }
        public PublicUserDto User { get; set; }
        public MediaAggregateDto Media { get; set; }

        public PostReactionSummaryDto ReactionSummary { get; set; }
    }

}

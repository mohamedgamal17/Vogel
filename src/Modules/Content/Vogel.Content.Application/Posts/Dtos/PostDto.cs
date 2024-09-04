﻿using Vogel.Application.Medias.Dtos;
using Vogel.Application.PostReactions.Dtos;
using Vogel.Application.Users.Dtos;
using Vogel.BuildingBlocks.Shared.Dtos;
namespace Vogel.Content.Application.Posts.Dtos
{
    public class PostDto : EntityDto<string>
    {
        public string Caption { get; set; }
        public string? UserId { get; set; }
        public string? MediaId { get; set; }
        public UserDto User { get; set; }
        public MediaAggregateDto Media { get; set; }
        public PostReactionSummaryDto ReactionSummary { get; set; }
    }

}

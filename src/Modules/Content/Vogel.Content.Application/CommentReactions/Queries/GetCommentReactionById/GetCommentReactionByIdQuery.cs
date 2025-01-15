using Microsoft.AspNetCore.Authorization;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.CommentReactions.Dtos;

namespace Vogel.Content.Application.CommentReactions.Queries.GetCommentReactionById
{
    [Authorize]
    public class GetCommentReactionByIdQuery : IQuery<CommentReactionDto>
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
        public string ReactionId { get; set; }
    }
}

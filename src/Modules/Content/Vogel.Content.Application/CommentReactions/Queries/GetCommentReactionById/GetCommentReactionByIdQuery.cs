using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Content.Application.CommentReactions.Dtos;

namespace Vogel.Content.Application.CommentReactions.Queries.GetCommentReactionById
{
    public class GetCommentReactionByIdQuery : IQuery<CommentReactionDto>
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
        public string ReactionId { get; set; }
    }
}

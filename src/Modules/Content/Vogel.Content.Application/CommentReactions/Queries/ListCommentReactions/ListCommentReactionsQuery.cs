using Minio.Helper;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.BuildingBlocks.Shared.Models;
using Vogel.Content.Application.CommentReactions.Dtos;

namespace Vogel.Content.Application.CommentReactions.Queries.ListCommentReactions
{
    public class ListCommentReactionsQuery : PagingParams, IQuery<Paging<CommentReactionDto>>
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
    }
}

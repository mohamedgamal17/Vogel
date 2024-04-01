using Vogel.Application.Comments.Dtos;
using Vogel.Application.Common.Interfaces;
using Vogel.Application.Common.Models;

namespace Vogel.Application.Comments.Queries
{
    public class ListCommentsQuery : PagingParams , IQuery<Paging<CommentAggregateDto>>
    {
        public string PostId { get; set; }
    }


    public class GetCommentQuery : IQuery<CommentAggregateDto>
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
    }
}

using Vogel.Application.Comments.Dtos;
using Vogel.Application.Common.Models;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.MongoDb.Entities.Common;

namespace Vogel.Application.Comments.Queries
{
    public class ListCommentsQuery : PagingParams , IQuery<Paging<CommentDto>>
    {
        public string PostId { get; set; }
    }


    public class GetCommentQuery : IQuery<CommentDto>
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
    }

    public class GetSubCommentsQuery : PagingParams, IQuery<Paging<CommentDto>>
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
    }
}

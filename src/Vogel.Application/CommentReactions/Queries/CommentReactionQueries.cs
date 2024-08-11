using Vogel.Application.CommentReactions.Dtos;
using Vogel.Application.Common.Models;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.MongoDb.Entities.Common;

namespace Vogel.Application.CommentReactions.Queries
{
    public class ListCommentReactionQuery : PagingParams, IQuery<Paging<CommentReactionDto>>
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
    }


    public class GetCommentReactionQuery : IQuery<CommentReactionDto>
    {
        public string Id { get; set; }

        public string PostId { get; set; }
        public string CommentId { get; set; }
    }
}

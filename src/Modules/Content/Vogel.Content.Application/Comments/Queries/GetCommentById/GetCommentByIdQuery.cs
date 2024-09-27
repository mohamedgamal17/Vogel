using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.Comments.Dtos;
namespace Vogel.Content.Application.Comments.Queries.GetCommentById
{
    public class GetCommentByIdQuery : IQuery<CommentDto>
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
    }
}

using MongoDB.Driver;
using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.Comments.Dtos;
using Vogel.Content.Domain.Posts;

namespace Vogel.Content.Application.Comments.Commands.UpdateComment
{
    public class UpdateCommentCommand : ICommand<CommentDto>
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
        public string Content { get; set; }
 
    }
}

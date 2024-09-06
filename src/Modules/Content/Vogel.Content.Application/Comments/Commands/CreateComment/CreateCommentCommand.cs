using Vogel.BuildingBlocks.Application.Requests;
using Vogel.Content.Application.Comments.Dtos;

namespace Vogel.Content.Application.Comments.Commands.CreateComment
{
    public class CreateCommentCommand : ICommand<CommentDto>
    {
        public string Content { get; set; }
        public string PostId { get; set; }
        public string? CommentId { get; set; }
    }
}

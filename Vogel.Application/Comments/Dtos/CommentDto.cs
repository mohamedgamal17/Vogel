using Vogel.Application.Common.Dtos;

namespace Vogel.Application.Comments.Dtos
{
    public class CommentDto : EntityDto
    {
        public string Content { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
    }
}

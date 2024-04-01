using Vogel.Application.Common.Dtos;
using Vogel.Application.Users.Dtos;
namespace Vogel.Application.Comments.Dtos
{
    public class CommentAggregateDto : EntityDto
    {
        public string Content { get; set; }
        public string UserId { get; set; }

        public string PostId { get; set; }
        public PublicUserDto User { get; set; }
    }
}

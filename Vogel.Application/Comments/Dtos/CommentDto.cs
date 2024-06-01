using Vogel.Application.Users.Dtos;
using Vogel.BuildingBlocks.Application.Dtos;
namespace Vogel.Application.Comments.Dtos
{
    public class CommentDto : EntityDto<string>
    {
        public string Content { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
        public PublicUserDto User { get; set; }
    }
}

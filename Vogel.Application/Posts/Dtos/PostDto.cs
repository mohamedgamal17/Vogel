using Vogel.Application.Common.Dtos;

namespace Vogel.Application.Posts.Dtos
{
    public class PostDto : EntityDto
    {
        public string Caption { get; set; }
        public string? MediaId { get; set; }
        public string UserId { get; set; }
    }
}

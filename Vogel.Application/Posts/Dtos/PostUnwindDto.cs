using Vogel.Application.Common.Dtos;
using Vogel.Application.Medias.Dtos;
using Vogel.Application.Users.Dtos;
namespace Vogel.Application.Posts.Dtos
{
    public class PostUnwindDto : EntityDto
    {
        public string Caption { get; set; }
        public string? MediaId { get; set; }
        public string UserId { get; set; }
        public MediaDto Media { get; set; }
        public UserDto User { get; set; }
    }

    public class PostLookupDto : EntityDto
    {
        public string Caption { get; set; }
        public string? MediaId { get; set; }
        public string UserId { get; set; }
        public MediaDto Media { get; set; }
        public UserDto User { get; set; }
    }
}

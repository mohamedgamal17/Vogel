using Vogel.Application.Common.Dtos;
using Vogel.Application.Medias.Dtos;
using Vogel.Application.Users.Dtos;
namespace Vogel.Application.Posts.Dtos
{
    public class PostAggregateDto : EntityDto
    {
        public string Caption { get; set; }
        public PublicUserDto User { get; set; }
        public MediaAggregateDto Media { get; set; }
    }

}

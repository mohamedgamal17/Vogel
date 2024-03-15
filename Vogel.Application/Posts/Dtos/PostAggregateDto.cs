using Vogel.Application.Common.Dtos;
using Vogel.Application.Medias.Dtos;
using Vogel.Application.Users.Dtos;
using Vogel.Domain;

namespace Vogel.Application.Posts.Dtos
{
    public class PostAggregateDto : EntityDto
    {
        public string Caption { get; set; }
        public PostUserDto User { get; set; }
        public MediaAggregateDto Media { get; set; }
    }
    public class PostUserDto : EntityDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
    }
}

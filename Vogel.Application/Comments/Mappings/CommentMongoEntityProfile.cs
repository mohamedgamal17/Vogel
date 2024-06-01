using AutoMapper;
using Vogel.Domain.Posts;
using Vogel.MongoDb.Entities.Comments;

namespace Vogel.Application.Comments.Mappings
{
    public class CommentMongoEntityProfile : Profile
    {
        public CommentMongoEntityProfile()
        {
            CreateMap<Comment, CommentMongoEntity>().ReverseMap();
        }
    }
}

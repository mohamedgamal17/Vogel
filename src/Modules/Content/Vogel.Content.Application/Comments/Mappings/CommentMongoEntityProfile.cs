using AutoMapper;
using Vogel.Content.Domain.Comments;
using Vogel.Content.MongoEntities.Comments;
namespace Vogel.Content.Application.Comments.Mappings
{
    public class CommentMongoEntityProfile : Profile
    {
        public CommentMongoEntityProfile()
        {
            CreateMap<Comment, CommentMongoEntity>()
                .ReverseMap();
        }
    }
}

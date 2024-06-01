using AutoMapper;
using Vogel.Domain.Posts;
using Vogel.MongoDb.Entities.Posts;

namespace Vogel.Application.Posts.Mappings
{
    public class PostMongoEntityProfile : Profile
    {
        public PostMongoEntityProfile()
        {
            CreateMap<Post, PostMongoEntity>();
        }
    }
}

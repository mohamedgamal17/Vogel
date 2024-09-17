using AutoMapper;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.Posts;

namespace Vogel.Content.Application.Posts.Mappings
{
    public class PostMongoEntityProfile : Profile
    {
        public PostMongoEntityProfile()
        {
            CreateMap<Post, PostMongoEntity>();
        }
    }
}

using AutoMapper;
using Vogel.Domain.Posts;
using Vogel.MongoDb.Entities.PostReactions;

namespace Vogel.Application.PostReactions.Mappings
{
    public class PostReactionMongoEntityProfile : Profile
    {
        public PostReactionMongoEntityProfile()
        {
            CreateMap<PostReaction, PostReactionMongoEntity>();
        }
    }
}

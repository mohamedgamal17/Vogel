using AutoMapper;
using Vogel.Content.Domain.Posts;
using Vogel.Content.MongoEntities.PostReactions;
namespace Vogel.Content.Application.PostReactions.Mappings
{
    public class PostReactionMongoEntityProfile : Profile
    {
        public PostReactionMongoEntityProfile()
        {
            CreateMap<PostReaction, PostReactionMongoEntity>();
        }
    }
}

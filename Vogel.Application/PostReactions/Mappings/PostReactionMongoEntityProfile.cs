using AutoMapper;
using Vogel.Domain.Posts;
using Vogel.MongoDb.Entities.Reactions;

namespace Vogel.Application.PostReactions.Mappings
{
    public class PostReactionMongoEntityProfile : Profile
    {
        public PostReactionMongoEntityProfile()
        {
            CreateMap<PostReaction, ReactionMongoEntity>();
        }
    }
}
